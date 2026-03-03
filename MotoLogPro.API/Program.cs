using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MotoLogPro.API.Middleware;
using MotoLogPro.Domain.Entities;
using MotoLogPro.Domain.Interfaces;
using MotoLogPro.Infrastructure.Data;
using MotoLogPro.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// --- 1. DATABASE ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("MotoLogPro.Infrastructure")));

builder.Services.AddScoped<IMotorcycleService, MotorcycleService>();

// --- 2. AUTH ---
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// --- 3. API ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.ToString());
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Inserisci il token JWT",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// --- 4. PROBLEM DETAILS (supporto RFC 7807 nativo) ---
builder.Services.AddProblemDetails();

var app = builder.Build();

// --- 5. PIPELINE ---

// Il middleware globale va PRIMA di tutto il resto
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityApi<ApplicationUser>();
app.MapControllers();

app.Run();