using OpencastReplacement.Models;
using System.Collections.Concurrent;

namespace OpencastReplacement.Services
{
    public class FileQueueMonitor
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<FileQueueMonitor> _logger;
        private readonly IFfmpegWrapper _ffmpegWrapper;
        ConcurrentQueue<Video> _queueForUpload;

        public FileQueueMonitor(IBackgroundTaskQueue taskQueue, ILogger<FileQueueMonitor> logger, IFfmpegWrapper wrapper)
        {
            _taskQueue = taskQueue;
            _logger = logger;
            _ffmpegWrapper = wrapper;
            _queueForUpload = new();
        }

        public async ValueTask QueueFileForEncoding(Video video)
        {
            _queueForUpload.Enqueue(video);
            await _taskQueue.QueueBackgroundWorkItemAsync(BuildWorkItem);
        }

        private async ValueTask BuildWorkItem(CancellationToken cancellationToken)
        {
            Video? video;
            if(_queueForUpload.TryDequeue(out video))
            {
                _logger.LogInformation($"Queued Encoding is starting: {video.FileName}");
                try
                {
                    string? message;
                    var success = await _ffmpegWrapper.StartEncoding(video, out message);
                    if (success)
                    {
                        _logger.LogInformation($"Queued encoding for {video.FileName} was successful");
                    } else
                    {
                        _logger.LogWarning($"Queued encoding for {video.FileName} failed, reason: {message}");
                    }
                } catch (Exception ex)
                {
                    _logger.LogError($"Queued encoding exception raised: {ex.Message}");
                }
            }
        }
    }
}
