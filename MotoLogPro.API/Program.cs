using Microsoft.EntityFrameworkCore;
using MotoLogPro.Infrastructure.Data;  // <--- QUESTA è quella che ti manca!
using MotoLogPro.Domain.Entities;      // <--- Serve per ApplicationUser

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURAZIONE DATABASE ---
// Diciamo all'API di usare SQL Server tramite il DbContext che sta in Infrastructure
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        // IMPORTANTE: Le migrazioni si trovano nel progetto Infrastructure, non qui!
        b => b.MigrationsAssembly("MotoLogPro.Infrastructure")));

// --- 2. CONFIGURAZIONE IDENTITY (AUTH) ---
// Abilita l'autenticazione e l'autorizzazione
builder.Services.AddAuthorization();

// Aggiunge gli endpoint preimpostati di Identity (Login, Register, ecc.)
// e li collega al nostro DbContext
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// --- 3. SERVIZI API ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 4. PIPELINE DI GESTIONE RICHIESTE ---

// Configura Swagger (solo in sviluppo)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization(); // Abilita il controllo dei permessi

// Mappa gli endpoint standard di Identity (es. /register, /login)
app.MapIdentityApi<ApplicationUser>();

app.MapControllers();

app.Run();