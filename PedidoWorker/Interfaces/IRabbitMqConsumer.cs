using System.Threading;
using System.Threading.Tasks;

namespace PedidoWorker.Interfaces
{
    public interface IRabbitMqConsumer
    {
        Task StartAsync(CancellationToken cancellationToken);
    }
}
