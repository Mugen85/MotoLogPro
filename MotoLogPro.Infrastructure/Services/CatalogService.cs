using Microsoft.EntityFrameworkCore;
using MotoLogPro.Infrastructure.Data;
using MotoLogPro.Shared.DTOs;

namespace MotoLogPro.Infrastructure.Services
{
    public class CatalogService(ApplicationDbContext context) : ICatalogService
    {
        public async Task<IEnumerable<BrandDto>> GetBrandsAsync()
        {
            return await context.Brands
                .AsNoTracking()
                .OrderBy(b => b.Name)
                .Select(b => new BrandDto { Id = b.Id, Name = b.Name })
                .ToListAsync();
        }

        public async Task<IEnumerable<BikeModelDto>> GetModelsByBrandAsync(int brandId)
        {
            return await context.BikeModels
                .AsNoTracking()
                .Where(m => m.BrandId == brandId)
                .OrderBy(m => m.Name)
                .Select(m => new BikeModelDto { Id = m.Id, Name = m.Name, BrandId = m.BrandId })
                .ToListAsync();
        }
    }
}