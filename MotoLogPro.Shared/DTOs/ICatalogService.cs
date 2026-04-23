namespace MotoLogPro.Shared.DTOs
{
    public interface ICatalogService
    {
        Task<IEnumerable<BrandDto>> GetBrandsAsync();
        Task<IEnumerable<BikeModelDto>> GetModelsByBrandAsync(int brandId);
    }
}