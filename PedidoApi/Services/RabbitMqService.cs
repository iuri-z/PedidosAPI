using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;

namespace PedidoApi.Services
{
    public class RabbitMqService
    {
        private readonly ConnectionFactory _factory;

        public RabbitMqService(IConfiguration configuration)
        {
            _factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:Host"] ?? "rabbitmq",
                UserName = "guest",
                Password = "guest"
            };
        }

        public async Task PublishAsync(string queue, string message)
        {
            var connection = await _factory.CreateConnectionAsync();
            await using (connection.ConfigureAwait(false))
            {
                var channel = await connection.CreateChannelAsync();
                await using (channel.ConfigureAwait(false))
                {
                    await channel.QueueDeclareAsync(queue, durable: false, exclusive: false, autoDelete: false);

                    var body = Encoding.UTF8.GetBytes(message);

                    await channel.BasicPublishAsync(
                        exchange: "",
                        routingKey: queue,
                        body: body
                    );
                }
            }
        }
    }
}
