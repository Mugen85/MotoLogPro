using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoLogPro.Infrastructure.Services;
using MotoLogPro.Shared.DTOs;

namespace MotoLogPro.API.Controllers
{
    [Route("api/catalog")]
    [ApiController]
    /*[Authorize]*/ // Rimuovilo solo se vuoi che chiunque possa leggere le marche senza login
    public class CatalogController(ICatalogService catalogService) : ControllerBase
    {
        [HttpGet("brands")]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetBrands()
        {
            var brands = await catalogService.GetBrandsAsync();
            return Ok(brands);
        }

        [HttpGet("brands/{brandId}/models")]
        public async Task<ActionResult<IEnumerable<BikeModelDto>>> GetModels(int brandId)
        {
            var models = await catalogService.GetModelsByBrandAsync(brandId);
            return Ok(models);
        }
    }
}