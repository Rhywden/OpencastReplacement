namespace OpencastReplacement.Services
{
    public class QueuedHostedService : BackgroundService
    {
        private readonly ILogger<QueuedHostedService> Logger;
        public IBackgroundTaskQueue TaskQueue { get; }

        public QueuedHostedService(IBackgroundTaskQueue taskQueue, ILogger<QueuedHostedService> logger)
        {
            Logger = logger;
            TaskQueue = taskQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.LogInformation("Queued Hosted Service is running.");
            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await TaskQueue.DequeueAsync(stoppingToken);
                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    Logger?.LogError(ex, "Error executing {WorkItem}", nameof(workItem));
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            Logger.LogInformation("Queued Hosted Service is stopping.");
            await base.StopAsync(stoppingToken);
        }
    }
}
