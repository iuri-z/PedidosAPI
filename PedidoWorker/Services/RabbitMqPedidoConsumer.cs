using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PedidoWorker.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PedidoWorker.Services
{
    public class RabbitMqPedidoConsumer : IRabbitMqConsumer
    {
        private readonly IPedidoProcessor _processor;
        private readonly ILogger<RabbitMqPedidoConsumer> _logger;
        private readonly ConnectionFactory _factory;
        private IConnection _connection;
        private IChannel _channel;

        private const string QueueName = "pedidos_queue";

        public RabbitMqPedidoConsumer(IPedidoProcessor processor, ILogger<RabbitMqPedidoConsumer> logger)
        {
            _processor = processor;
            _logger = logger;

            _factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await ConectarRabbitMqComRetry(cancellationToken);

            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var mensagem = Encoding.UTF8.GetString(ea.Body.ToArray());

                if (!Guid.TryParse(mensagem, out var pedidoId))
                {
                    _logger.LogWarning("Mensagem inválida recebida: {Mensagem}", mensagem);
                    return;
                }

                await _processor.ProcessarAsync(pedidoId);
            };

            await _channel.BasicConsumeAsync(
                queue: QueueName,
                autoAck: true,
                consumer: consumer);

            _logger.LogInformation("Worker escutando fila...");

            await Task.Delay(Timeout.Infinite, cancellationToken);
        }

        private async Task ConectarRabbitMqComRetry(CancellationToken cancellationToken)
        {
            var tentativas = 10;
            var atraso = TimeSpan.FromSeconds(3);

            for (int i = 1; i <= tentativas; i++)
            {
                try
                {
                    _connection = await _factory.CreateConnectionAsync();
                    _channel = await _connection.CreateChannelAsync();

                    _logger.LogInformation("Conectado ao RabbitMQ na tentativa {Tentativa}.", i);
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        ex,
                        "Falha ao conectar no RabbitMQ (tentativa {Tentativa}/{Total}).",
                        i,
                        tentativas);

                    if (i == tentativas)
                        throw;

                    await Task.Delay(atraso, cancellationToken);
                }
            }
        }
    }
}
