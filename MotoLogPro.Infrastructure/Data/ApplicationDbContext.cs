using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MotoLogPro.Domain.Entities;

namespace MotoLogPro.Infrastructure.Data
{
    // 1. DEVE ESSERE PUBLIC per essere vista dall'API
    // 2. DEVE ERIDITARE da IdentityDbContext per gestire gli utenti
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {

        // Tabella delle Moto
        public DbSet<Motorcycle> Motorcycles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configurazioni Fluent API per le relazioni
            builder.Entity<Motorcycle>(entity =>
            {
                // Indice univoco sul telaio (VIN)
                entity.HasIndex(m => m.Vin).IsUnique();

                // Se cancello l'utente, cancello le sue moto a cascata
                entity.HasOne(m => m.User)
                      .WithMany(u => u.Motorcycles)
                      .HasForeignKey(m => m.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}