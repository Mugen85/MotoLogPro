using MotoLogPro.Shared.DTOs;
using System.Net.Http.Json;

namespace MotoLogPro.Client.Services
{
    public class CatalogService(HttpClient httpClient) : ICatalogService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<IEnumerable<BrandDto>> GetBrandsAsync()
        {
            // Endpoint pubblico, non serve il token di autorizzazione!
            var brands = await _httpClient.GetFromJsonAsync<IEnumerable<BrandDto>>("api/catalog/brands");
            return brands ?? [];
        }

        public async Task<IEnumerable<BikeModelDto>> GetModelsByBrandAsync(int brandId)
        {
            var models = await _httpClient.GetFromJsonAsync<IEnumerable<BikeModelDto>>($"api/catalog/brands/{brandId}/models");
            return models ?? [];
        }
    }
}