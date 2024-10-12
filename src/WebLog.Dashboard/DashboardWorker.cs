
namespace WebLog.Dashboard
{
    internal class DashboardWorker(ILogger<DashboardWorker> logger) : BackgroundService
    {
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Server has started");

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Server has stopped");

            return Task.CompletedTask;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
