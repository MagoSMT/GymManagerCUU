using Microsoft.EntityFrameworkCore;
using GymManager.Models;

namespace GymManager.Data
{
    // Hereda de DbContext para conectar y trabajar con la base de datos
    public class GymDbContext : DbContext
    {
        public GymDbContext(DbContextOptions<GymDbContext> options)
            : base(options)
        {
        }

        // Representan las tablas de la base de datos
        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Socio> Socios { get; set; }
        public DbSet<Plan> Planes { get; set; }
        public DbSet<Suscripcion> Suscripciones { get; set; }
        public DbSet<Pago> Pagos { get; set; }
    }
}
