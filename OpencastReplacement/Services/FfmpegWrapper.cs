using FFMpegCore;
using FFMpegCore.Enums;
using Fluxor;
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

        public FfmpegWrapper(IWebHostEnvironment env, ILogger<FfmpegWrapper> log, ConfigurationWrapper conf, ConversionProgressEvent evt)
        {
            hostingEnv = env;
            logger = log;
            configurationManager = conf.ConfigurationManager;
            conversionProgressEvent = evt;
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
                logger.LogInformation($"Progress on encode: {p}");
            });

            GlobalFFOptions.Configure(new FFOptions { BinaryFolder = configurationManager["ffmpeg:exepath"] });
            string input;
            string output;
            if (hostingEnv.IsDevelopment())
            {
                input = Path.Combine(hostingEnv.ContentRootPath,
                        "wwwroot", "temp", video.FileName);
                output = Path.Combine(hostingEnv.ContentRootPath,
                        "wwwroot", "uploads", video.FileName);
            } else
            {
                input = configurationManager["ffmpeg:temppath"];
                output = configurationManager["ffmpeg:storepath"];
            }

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
            } catch (Exception e) {
                logger.LogCritical($"FFMpeg threw error: {e.InnerException}");
            }
            return true;
        }
    }
}
