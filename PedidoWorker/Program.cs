using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PedidoWorker;
using Shared.Data;
using System;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PedidosDb")));

builder.Services.AddHostedService<Worker>();

var app = builder.Build();
app.Run();
