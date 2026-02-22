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
                    OwnerName = m.User!.FullName
                })
                .ToListAsync();
        }

        public async Task<VehicleDto> CreateAsync(string userId, CreateMotorcycleDto dto)
        {
            var moto = new Motorcycle
            {
                Brand = dto.Brand,
                Model = dto.Model,
                Year = dto.Year,
                Vin = dto.Vin,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Motorcycles.Add(moto);
            await _context.SaveChangesAsync();

            return new VehicleDto
            {
                Id = moto.Id,
                Brand = moto.Brand,
                Model = moto.Model,
                Year = moto.Year,
                Vin = moto.Vin
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

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string userId, int id)
        {
            var moto = await _context.Motorcycles
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (moto is null) return false;

            _context.Motorcycles.Remove(moto);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}