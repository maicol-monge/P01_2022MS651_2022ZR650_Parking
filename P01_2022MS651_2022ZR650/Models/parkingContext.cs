using Microsoft.EntityFrameworkCore;

namespace P01_2022MS651_2022ZR650.Models
{
    public class parkingContext : DbContext
    {
        public parkingContext(DbContextOptions<parkingContext> options) : base(options)
        {
        }
        public DbSet<Usuario> usuarios { get; set; }
        public DbSet<Sucursal> sucursales { get; set; }

    }
}
