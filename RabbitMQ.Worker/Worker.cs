using RabbitMQ.Worker.Services;

namespace RabbitMQ.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IRabbitMqConsumer consumer;
        public Worker(ILogger<Worker> logger, IRabbitMqConsumer consumer)
        {
            _logger = logger;
            this.consumer = consumer;
            this.consumer.Call();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}
