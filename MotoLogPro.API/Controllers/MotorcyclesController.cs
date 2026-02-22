using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoLogPro.Domain.Interfaces;
using MotoLogPro.Shared.DTOs;
using System.Security.Claims; // Serve per leggere l'ID utente dal Token

namespace MotoLogPro.API.Controllers
{
    [Route("api/vehicles")]
    [ApiController]
    [Authorize]
    public class MotorcyclesController(IMotorcycleService motorcycleService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetMotorcycles()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await motorcycleService.GetByUserAsync(userId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<VehicleDto>> PostMotorcycle(CreateMotorcycleDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
                var created = await motorcycleService.CreateAsync(userId, dto);
                return CreatedAtAction(nameof(GetMotorcycles), new { id = created.Id }, created);
            }
            catch (DbUpdateException)
            {
                return Conflict("Il VIN inserito è già presente nel sistema.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMotorcycle(int id, CreateMotorcycleDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var updated = await motorcycleService.UpdateAsync(userId, id, dto);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMotorcycle(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var deleted = await motorcycleService.DeleteAsync(userId, id);
            return deleted ? NoContent() : NotFound();
        }
    }
}