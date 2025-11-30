using FFMpegCore;
using FFMpegCore.Enums;
using OpencastReplacement.Data;
using OpencastReplacement.Models;
using OpencastReplacement.Store;
using RudderSingleton;

namespace OpencastReplacement.Services
{
    public class FfmpegWrapper : IFfmpegWrapper
    {
        private IWebHostEnvironment hostingEnv;
        private ILogger<FfmpegWrapper> logger;
        private ConfigurationManager configurationManager;
        private IMongoConnection _connection;
        private readonly Store<AppState> _store;

        public FfmpegWrapper(IWebHostEnvironment env,
            ILogger<FfmpegWrapper> log, 
            ConfigurationWrapper conf, 
            IMongoConnection connection,
            Store<AppState> store)
        {
            hostingEnv = env;
            logger = log;
            configurationManager = conf.ConfigurationManager;
            _connection = connection;
            _store = store;
        }
        public Task<bool> CancelEncoding(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> StartEncoding(Video video)
        {
            GlobalFFOptions.Configure(new FFOptions { BinaryFolder = configurationManager["ffmpeg:exepath"] });
            string? input;
            string? output;
            var newFilename = Path.ChangeExtension(video.FileName, ".mp4");
    
            if (System.Environment.GetEnvironmentVariable("VIDEO_STORAGE") == "external")
            {
                input = System.Environment.GetEnvironmentVariable("VIDEO_TEMP_PATH") + "/" + video.FileName;
                logger.LogInformation($"Exe path: {configurationManager["ffmpeg:exepath"]}");
                output = System.Environment.GetEnvironmentVariable("VIDEO_STORAGE_PATH") + "/" + newFilename;
                if (input is null || output is null) throw new Exception("Path to video storage not set in appsettings.json"); 
            }
            else
            {
                input = Path.Combine(hostingEnv.ContentRootPath,
                            "wwwroot", "temp", video.FileName);
                output = Path.Combine(hostingEnv.ContentRootPath,
                            "wwwroot", "uploads", newFilename);
            }
    
            var conversion = new Conversion
            {
                Filename = video.Public ? video.FileName : "Nicht öffentlich",
                Progress = 0,
                HasStarted = true,
                VideoId = video.Id
            };
    
            IMediaAnalysis? media = null;
            try
            {
                media = await FFProbe.AnalyseAsync(input);
            } 
            catch (Exception e)
            {
                logger.LogCritical($"FFProbe threw error: {e.Message}: {e.InnerException}");
            }
    
            _store.Put(new Actions.UpdateConversion.Request(conversion));
    
            // Determine if this is an audio-only file
            bool isAudioOnly = IsAudioOnlyFile(media, video.FileName);
    
            Action<double> progressHandler = new Action<double>(p =>
            {
                var convProgress = conversion with
                {
                    HasStarted = true,
                    Progress = p
                };                
                _store.Put(new Actions.UpdateConversion.Request(convProgress));
                logger.LogInformation($"Progress on encode: {p}");
            });
    
            Action<string> errorHandler = new Action<string>(p =>
            {
                string[] ary = p.Split(' ');
                string[]? pAry = null;
                if (media is null)
                {
                    var convProgress = conversion with
                    {
                        HasStarted = true,
                        Progress = 0.5
                    };
                    _store.Put(new Actions.UpdateConversion.Request(convProgress));
                }
                else
                {
                    for (int i = 0; i < ary.Length; i++)
                    {
                        if (ary[i] != string.Empty)
                        {
                            pAry = ary[i].Split('=');
                            if (pAry[0].Equals("time"))
                            {
                                TimeSpan timeComplete;
                                var valid = TimeSpan.TryParse(pAry[1], out timeComplete);
                                if (!valid)
                                {
                                    timeComplete = media.Duration;
                                }
                                TimeSpan timeLeft = media.Duration - timeComplete;
                                double secondsLeft = timeLeft.TotalSeconds;
                                double percentage = Math.Round(100 - (secondsLeft * 100 / media.Duration.TotalSeconds), 1);

                                var convProgress = conversion with
                                {
                                    HasStarted = true,
                                    Progress = percentage
                                };
                                _store.Put(new Actions.UpdateConversion.Request(convProgress));
                                logger.LogInformation($"Progress on encode: {p}");
                            }
                        }
                    }
                }
            });

            try 
            {
                bool result;
        
                if (isAudioOnly)
                {
                    // Path to static image
                    string imagePath = Path.Combine(hostingEnv.ContentRootPath, "wwwroot", "images", "schallplattenspieler.jpg");
            
                    // Audio-to-video conversion with static image
                    // Use -t to explicitly set duration to match audio length
                    result = await FFMpegArguments
                        .FromFileInput(imagePath, false, options => options
                            .Loop(1)
                            .WithCustomArgument("-framerate 1"))
                        .AddFileInput(input)
                        .OutputToFile(output, false, options => options
                            .WithVideoCodec(VideoCodec.LibX264)
                            .WithConstantRateFactor(23)
                            .WithAudioCodec(AudioCodec.Aac)
                            .WithCustomArgument("-shortest")
                            .WithCustomArgument("-pix_fmt yuv420p")
                            .WithCustomArgument($"-t {media?.Duration.TotalSeconds ?? 0}")
                            .WithFastStart())
                        .NotifyOnProgress(progressHandler, media?.Duration ?? TimeSpan.Zero)
                        .NotifyOnError(errorHandler)
                        .ProcessAsynchronously();
                }
                else
                {
                    // Standard video conversion
                    result = await FFMpegArguments
                        .FromFileInput(input)
                        .OutputToFile(output, false, options => options
                            .WithVideoCodec(VideoCodec.LibX264)
                            .WithConstantRateFactor(23)
                            .WithAudioCodec(AudioCodec.Aac)
                            .WithFastStart())
                        .NotifyOnProgress(progressHandler, media?.Duration ?? TimeSpan.Zero)
                        .NotifyOnError(errorHandler)
                        .ProcessAsynchronously();
                }
        
                File.Delete(input);
                _store.Put(new Actions.DeleteConversion.Request(conversion));

                var vid = video with
                {
                    Duration = media?.Duration ?? TimeSpan.Zero,
                    Height = isAudioOnly ? 720 : (media?.PrimaryVideoStream?.Height ?? 0),
                    Width = isAudioOnly ? 1280 : (media?.PrimaryVideoStream?.Width ?? 0),
                    FileName = newFilename
                };
                _store.Put(new Actions.AddVideo.Request(videoToBeAdded: vid));
            } 
            catch (Exception e) 
            {
                File.Delete(input);
                _store.Put(new Actions.DeleteConversion.Request(conversion));
                logger.LogCritical($"FFMpeg threw error: {e.HResult} {e.Message} : {e.InnerException}");
            }
    
            return true;
        }

        private bool IsAudioOnlyFile(IMediaAnalysis? media, string fileName)
        {
            // Check by file extension
            string[] audioExtensions = { ".mp3", ".ogg", ".wma", ".m4a", ".aac", ".flac", ".wav" };
            string extension = Path.GetExtension(fileName).ToLowerInvariant();
    
            if (audioExtensions.Contains(extension))
                return true;
    
            // Fallback: check if media has no video stream
            if (media != null && media.PrimaryVideoStream == null && media.PrimaryAudioStream != null)
                return true;
    
            return false;
        }
    }
}
