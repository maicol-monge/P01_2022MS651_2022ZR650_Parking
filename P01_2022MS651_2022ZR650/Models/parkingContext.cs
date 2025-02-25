using Microsoft.EntityFrameworkCore;

namespace P01_2022MS651_2022ZR650.Models
{
    public class parkingContext : DbContext
    {
        public parkingContext(DbContextOptions<parkingContext> options) : base(options)
        {
        }
        public DbSet<Usuario> usuario { get; set; }
        public DbSet<Sucursal> sucursal { get; set; }
        public DbSet<Espacio_parqueo> espacio_parqueo { get; set; }
        public DbSet<Reserva> reserva { get; set; }

    }
}
