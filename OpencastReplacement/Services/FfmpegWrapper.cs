using FFMpegCore;
using FFMpegCore.Enums;
using Fluxor;
using OpencastReplacement.Data;
using OpencastReplacement.Events;
using OpencastReplacement.Models;

namespace OpencastReplacement.Services
{
    public class FfmpegWrapper : IFfmpegWrapper
    {
        private IWebHostEnvironment hostingEnv;
        private ILogger<FfmpegWrapper> logger;
        private ConfigurationManager configurationManager;
        private ConversionProgressEvent conversionProgressEvent;
        private VideoAddedEvent videoAddedEvent;
        private IDataRepository repository;
        private IMongoConnection _connection;

        public FfmpegWrapper(IWebHostEnvironment env, 
            ILogger<FfmpegWrapper> log, 
            ConfigurationWrapper conf, 
            ConversionProgressEvent evt,
            VideoAddedEvent _videoAddedEvent,
            IDataRepository repo, 
            IMongoConnection connection)
        {
            hostingEnv = env;
            logger = log;
            configurationManager = conf.ConfigurationManager;
            conversionProgressEvent = evt;
            repository = repo;
            _connection = connection;
            videoAddedEvent = _videoAddedEvent;
        }
        public Task<bool> CancelEncoding(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> StartEncoding(Video video)
        {
            var evtArgs = new ConversionProgressEventArgs
            {
                Progress = 0
            };
            Action<double> progressHandler = new Action<double>(async p =>
            {
                evtArgs.Progress = p;
                await conversionProgressEvent.Update(evtArgs);

                var index = repository.Conversions.FindIndex(c => c.VideoId.Equals(video.Id));
                if (index >= 0)
                {
                    var newConv = repository.Conversions[index] with { HasStarted = true, Progress = p };
                    repository.Conversions[index] = newConv;
                }

                logger.LogInformation($"Progress on encode: {p}");
            });

            GlobalFFOptions.Configure(new FFOptions { BinaryFolder = configurationManager["ffmpeg:exepath"] });
            string input = Path.Combine(hostingEnv.ContentRootPath,
                        "wwwroot", "temp", video.FileName);
            string output = Path.Combine(hostingEnv.ContentRootPath,
                        "wwwroot", "uploads", video.FileName);

            var media = await FFProbe.AnalyseAsync(input);

            try {
                var result = await FFMpegArguments
                    .FromFileInput(input)
                    .OutputToFile(output, false, options => options
                        .WithVideoCodec(VideoCodec.LibX264)
                        .WithConstantRateFactor(23)
                        .WithAudioCodec(AudioCodec.Aac)
                        .WithFastStart())
                    .NotifyOnProgress(progressHandler, media.Duration)
                    .ProcessAsynchronously();
                var index = repository.Conversions.FindIndex(c => c.VideoId.Equals(video.Id));
                repository.Conversions.RemoveAt(index);
                await conversionProgressEvent.Update(evtArgs);
                var vid = video with
                {
                    Duration = media.Duration,
                    Height = media.PrimaryVideoStream!.Height,
                    Width = media.PrimaryVideoStream!.Width,
                };
                var coll = _connection.GetVideoCollection();
                await coll.InsertOneAsync(vid);
                repository.Videos.Add(vid);
                await videoAddedEvent.Update(true);
            } catch (Exception e) {
                logger.LogCritical($"FFMpeg threw error: {e.InnerException}");
            }
            return true;
        }
    }
}
