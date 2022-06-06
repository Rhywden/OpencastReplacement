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
            string input = Path.Combine(hostingEnv.ContentRootPath,
                        "wwwroot", "temp", video.FileName);
            string output = Path.Combine(hostingEnv.ContentRootPath,
                        "wwwroot", "uploads", video.FileName);

            var media = await FFProbe.AnalyseAsync(input);

            var conversion = new Conversion
            {
                Filename = video.Public ? video.FileName : "Nicht öffentlich",
                Progress = 0,
                HasStarted = true,
                VideoId = video.Id
            };
            _store.Put(new Actions.UpdateConversion.Request(conversion));
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
                for(int i = 0; i < ary.Length; i++)
                {
                    if (ary[i] != string.Empty)
                    {
                        pAry = ary[i].Split('=');
                        if (pAry[0].Equals("time"))
                        {
                            TimeSpan timeComplete = TimeSpan.Parse(pAry[1]);
                            TimeSpan timeLeft = media.Duration - timeComplete;
                            double secondsLeft = timeLeft.TotalSeconds;
                            double percentage =  Math.Round(100 - (secondsLeft * 100 / media.Duration.TotalSeconds),1);

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
            });

            try {
                var result = await FFMpegArguments
                    .FromFileInput(input)
                    .OutputToFile(output, false, options => options
                        .WithVideoCodec(VideoCodec.LibX264)
                        .WithConstantRateFactor(23)
                        .WithAudioCodec(AudioCodec.Aac)
                        .WithFastStart())
                    .NotifyOnProgress(progressHandler, media.Duration)
                    .NotifyOnError(errorHandler)
                    .ProcessAsynchronously();
                File.Delete(input);
                _store.Put(new Actions.DeleteConversion.Request(conversion));

                

                var vid = video with
                {
                    Duration = media.Duration,
                    Height = media.PrimaryVideoStream!.Height,
                    Width = media.PrimaryVideoStream!.Width,
                };
                _store.Put(new Actions.AddVideo.Request(videoToBeAdded: vid));
            } catch (Exception e) {
                logger.LogCritical($"FFMpeg threw error: {e.InnerException}");
            }
            return true;
        }
    }
}
