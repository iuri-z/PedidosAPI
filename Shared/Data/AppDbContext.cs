using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Shared.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        public DbSet<Pedido> Pedidos { get; set; }
    }
}
