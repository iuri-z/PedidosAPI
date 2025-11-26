using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Shared.Data;
using PedidoWorker.Interfaces;

namespace PedidoWorker.Services
{
    public class PedidoProcessor : IPedidoProcessor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PedidoProcessor> _logger;

        public PedidoProcessor(IServiceProvider serviceProvider, ILogger<PedidoProcessor> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task ProcessarAsync(Guid pedidoId)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var pedido = await db.Pedidos.FirstOrDefaultAsync(p => p.Id == pedidoId);

                if (pedido is null)
                {
                    _logger.LogWarning($"Pedido {pedidoId} não encontrado.");
                    return;
                }

                pedido.Status = "processado";
                await db.SaveChangesAsync();

                _logger.LogInformation($"Pedido {pedidoId} processado.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao processar pedido {pedidoId}.");
                throw;
            }
        }
    }
}
