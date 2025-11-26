using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PedidoWorker.Interfaces;

namespace PedidoWorker
{
    public class Worker : BackgroundService
    {
        private readonly IRabbitMqConsumer _consumer;
        private readonly ILogger<Worker> _logger;

        public Worker(IRabbitMqConsumer consumer, ILogger<Worker> logger)
        {
            _consumer = consumer;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker iniciando consumo...");
            await _consumer.StartAsync(stoppingToken);
        }
    }
}
