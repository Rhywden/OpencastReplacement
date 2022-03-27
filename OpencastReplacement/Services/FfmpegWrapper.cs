using FFMpegCore;
using FFMpegCore.Enums;
using OpencastReplacement.Models;

namespace OpencastReplacement.Services
{
    public class FfmpegWrapper : IFfmpegWrapper
    {
        private IWebHostEnvironment hostingEnv;
        private ILogger<FfmpegWrapper> logger;

        public FfmpegWrapper(IWebHostEnvironment env, ILogger<FfmpegWrapper> log)
        {
            hostingEnv = env;
            logger = log;
        }
        public Task<bool> CancelEncoding(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> StartEncoding(Video video)
        {
            Action<double> progressHandler = new Action<double>(p =>
            {
                logger.LogInformation($"Progress on encode: {p}");
            });

            GlobalFFOptions.Configure(new FFOptions { BinaryFolder = "C:\\ProgramData\\chocolatey\\lib\\ffmpeg\\tools\\ffmpeg\\bin" });
            var input = Path.Combine(hostingEnv.ContentRootPath,
                    "wwwroot", "temp", video.FileName);
            var output = Path.Combine(hostingEnv.ContentRootPath,
                    "wwwroot", "uploads", video.FileName);

            var media = await FFProbe.AnalyseAsync(input);

            try {
                var result = await FFMpegArguments
                    .FromFileInput(input)
                    .OutputToFile(output, false, options => options
                        .WithVideoCodec(VideoCodec.LibX264)
                        .WithConstantRateFactor(21)
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
