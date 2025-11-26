using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Data;
using Shared.Models;
using System;
using System.Text;

namespace PedidoWorker
{
    public class Worker : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ConnectionFactory _factory;

        public Worker(IServiceProvider services, IConfiguration config)
        {
            _services = services;

            _factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connection = await ConnectWithRetryAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync("pedidos_queue", false, false, false);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var texto = Encoding.UTF8.GetString(ea.Body.ToArray());
                var id = Guid.Parse(texto);

                using var scope = _services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var pedido = await db.Pedidos.FirstOrDefaultAsync(p => p.Id == id);

                if (pedido != null)
                {
                    pedido.Status = "processado";
                    await db.SaveChangesAsync();
                }

                Console.WriteLine($"Pedido {id} processado.");
            };

            await channel.BasicConsumeAsync("pedidos_queue", true, consumer);

            Console.WriteLine("Worker escutando fila...");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        // Worker tentava conectar antes do RabbitMQ aceitar conexão
        // Portanto, implementado retry exponencial
        private async Task<IConnection> ConnectWithRetryAsync()
        {
            var retries = 10;

            for (int i = 1; i <= retries; i++)
            {
                try
                {
                    return await _factory.CreateConnectionAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Tentativa {i}/{retries}: RabbitMQ indisponível ({ex.Message}).");

                    await Task.Delay(TimeSpan.FromSeconds(5)); // espera e tenta de novo
                }
            }

            throw new Exception("Não foi possível conectar ao RabbitMQ após várias tentativas.");
        }

    }
}
