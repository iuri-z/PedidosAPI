using System;
using System.Threading.Tasks;

namespace PedidoWorker.Interfaces
{
    public interface IPedidoProcessor
    {
        Task ProcessarAsync(Guid pedidoId);
    }
}
