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

        // Aggiungiamo le tabelle del dizionario
        public DbSet<Brand> Brands { get; set; }
        public DbSet<BikeModel> BikeModels { get; set; }

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

                // --- 1. CONFIGURAZIONE RELAZIONI ---
                builder.Entity<BikeModel>()
                    .HasOne(m => m.Brand)
                    .WithMany(b => b.Models)
                    .HasForeignKey(m => m.BrandId)
                    .OnDelete(DeleteBehavior.Cascade);

                // --- 2. DATA SEEDING (Dizionario Base da Officina) ---
                builder.Entity<Brand>().HasData(
                    new Brand { Id = 1, Name = "Yamaha" },
                    new Brand { Id = 2, Name = "Honda" },
                    new Brand { Id = 3, Name = "Ducati" },
                    new Brand { Id = 4, Name = "BMW" },
                    new Brand { Id = 5, Name = "KTM" }
                );

                builder.Entity<BikeModel>().HasData(
                    // Yamaha
                    new BikeModel { Id = 1, Name = "Ténéré 700", BrandId = 1 },
                    new BikeModel { Id = 2, Name = "MT-07", BrandId = 1 },
                    new BikeModel { Id = 3, Name = "Tracer 9", BrandId = 1 },
                    // Honda
                    new BikeModel { Id = 4, Name = "Africa Twin", BrandId = 2 },
                    new BikeModel { Id = 5, Name = "Transalp 750", BrandId = 2 },
                    new BikeModel { Id = 6, Name = "CBR600RR", BrandId = 2 },
                    // Ducati
                    new BikeModel { Id = 7, Name = "Multistrada V4", BrandId = 3 },
                    new BikeModel { Id = 8, Name = "DesertX", BrandId = 3 },
                    // BMW
                    new BikeModel { Id = 9, Name = "R 1250 GS", BrandId = 4 },
                    new BikeModel { Id = 10, Name = "F 850 GS", BrandId = 4 },
                    // KTM
                    new BikeModel { Id = 11, Name = "1290 Super Adventure", BrandId = 5 },
                    new BikeModel { Id = 12, Name = "890 Duke", BrandId = 5 });
            });

            // --- 3. GLOBAL QUERY FILTERS ---
            // Entity Framework filtrerà in automatico tutte le moto "cancellate logicamente"
            builder.Entity<Motorcycle>().HasQueryFilter(m => !m.IsDeleted);
        }      
            
    }

}