using Microsoft.EntityFrameworkCore;
using MotoLogPro.Domain.Entities;
using MotoLogPro.Domain.Interfaces;
using MotoLogPro.Infrastructure.Data;
using MotoLogPro.Shared.DTOs;

namespace MotoLogPro.Infrastructure.Services
{
    public class MotorcycleService(ApplicationDbContext context) : IMotorcycleService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<IEnumerable<VehicleDto>> GetByUserAsync(string userId)
        {
            return await _context.Motorcycles
                .Where(m => m.UserId == userId)
                .Select(m => new VehicleDto
                {
                    Id = m.Id,
                    Brand = m.Brand,
                    Model = m.Model,
                    Year = m.Year,
                    Vin = m.Vin,
                    LicensePlate = m.LicensePlate, // ← AGGIUNTO
                    OwnerName = m.User!.FullName
                })
                .ToListAsync();
        }

        public async Task<VehicleDto> CreateAsync(string userId, CreateMotorcycleDto dto)
        {
            // FAIL-FAST: Le sentinelle all'ingresso.
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);
            ArgumentNullException.ThrowIfNull(dto);

            // 1. CONTROLLO GLOBALE: Diciamo a EF Core di guardare anche sotto i teloni (tra le moto cancellate)
            var existingMoto = await _context.Motorcycles
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(m => m.Vin == dto.Vin && m.UserId == userId);

            if (existingMoto != null)
            {
                if (existingMoto.IsDeleted)
                {
                    // SCENARIO REALE: La moto era cancellata ma è tornata. La riattiviamo (Resurrezione).
                    existingMoto.IsDeleted = false;
                    existingMoto.Brand = dto.Brand;
                    existingMoto.Model = dto.Model;
                    existingMoto.Year = dto.Year;
                    existingMoto.LicensePlate = dto.LicensePlate;
                    // Nota: Non tocchiamo CreatedAt, così manteniamo la data originale del primo ingresso in officina!

                    _context.Motorcycles.Update(existingMoto);
                    await _context.SaveChangesAsync();

                    return new VehicleDto
                    {
                        Id = existingMoto.Id,
                        Brand = existingMoto.Brand,
                        Model = existingMoto.Model,
                        Year = existingMoto.Year,
                        Vin = existingMoto.Vin,
                        LicensePlate = existingMoto.LicensePlate
                    };
                }
                else
                {
                    // SCENARIO ERRORE: L'utente sta provando a registrare un telaio che è già nel suo garage, visibile e attivo.
                    throw new InvalidOperationException($"Una moto con Telaio {dto.Vin} è già presente e attiva nel garage.");
                }
            }

            // 2. SCENARIO NORMALE: Nessuna traccia di questo telaio nel DB. È una moto totalmente nuova.
            var moto = new Motorcycle
            {
                Brand = dto.Brand,
                Model = dto.Model,
                Year = dto.Year,
                Vin = dto.Vin,
                LicensePlate = dto.LicensePlate,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Motorcycles.Add(moto);
            await _context.SaveChangesAsync();

            return new VehicleDto
            {
                Id = moto.Id,
                Brand = moto.Brand,
                Model = moto.Model,
                Year = moto.Year,
                Vin = moto.Vin,
                LicensePlate = moto.LicensePlate
            };
        }

        public async Task<bool> UpdateAsync(string userId, int id, CreateMotorcycleDto dto)
        {
            var moto = await _context.Motorcycles
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (moto is null) return false;

            moto.Brand = dto.Brand;
            moto.Model = dto.Model;
            moto.Year = dto.Year;
            moto.Vin = dto.Vin;
            moto.LicensePlate = dto.LicensePlate; // ← AGGIUNTO

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string userId, int id)
        {
            var moto = await _context.Motorcycles
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (moto == null) return false;

            // SOFT DELETE: Invece di distruggere il record, lo marchiamo come cancellato
            moto.IsDeleted = true;

            _context.Motorcycles.Update(moto);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}