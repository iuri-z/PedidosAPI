using Microsoft.EntityFrameworkCore;
using Shared.Data;
using PedidoApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<RabbitMqService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("PedidosDb"),
        x => x.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
    )
);

var app = builder.Build();

// Swagger SEMPRE habilitado (tanto VS quanto Docker)
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

// Aplica migration automaticamente ao subir a API
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
