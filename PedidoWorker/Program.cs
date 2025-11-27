using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PedidoWorker;
using PedidoWorker.Interfaces;
using PedidoWorker.Services;
using Shared.Data;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                hostContext.Configuration.GetConnectionString("PedidosDb")
            )
        );

        // Registrar processor e consumer
        services.AddSingleton<IPedidoProcessor, PedidoProcessor>();
        services.AddSingleton<IRabbitMqConsumer, RabbitMqPedidoConsumer>();

        // Registrar host service
        services.AddHostedService<Worker>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

await host.RunAsync();
