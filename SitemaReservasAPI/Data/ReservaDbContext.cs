using Microsoft.EntityFrameworkCore;
using SistemaReservasAPI.Models;

namespace SistemaReservasAPI.Data
{
    public class ReservaDbContext : DbContext
    {
        public ReservaDbContext(DbContextOptions<ReservaDbContext> options) : base(options) { }

        public DbSet<Reserva> Reservas { get; set; }
    }
}