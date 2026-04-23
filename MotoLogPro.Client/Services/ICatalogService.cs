using MotoLogPro.Shared.DTOs;

namespace MotoLogPro.Client.Services
{
    public interface ICatalogService
    {
        Task<IEnumerable<BrandDto>> GetBrandsAsync();
        Task<IEnumerable<BikeModelDto>> GetModelsByBrandAsync(int brandId);
    }
}