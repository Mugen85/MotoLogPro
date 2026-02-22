using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoLogPro.Domain.Entities;
using MotoLogPro.Infrastructure.Data;
using MotoLogPro.Shared.DTOs;
using System.Security.Claims; // Serve per leggere l'ID utente dal Token

namespace MotoLogPro.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // 🔒 Protegge tutto il controller: serve il Token JWT!
    public class MotorcyclesController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

        // GET: api/Motorcycles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetMotorcycles()
        {
            // 1. "Chi è l'utente che sta chiamando?"
            // Estraiamo l'ID utente dal Token JWT
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Utente non riconosciuto");
            }

            // 2. Query al DB filtrata per utente
            // "Dammi solo le moto di QUESTO utente"
            var motos = await _context.Motorcycles
                                      .Where(m => m.UserId == userId)
                                      .ToListAsync();

            // 3. Mappatura Entity -> DTO
            var dtos = motos.Select(m => new VehicleDto
            {
                Id = m.Id,
                Brand = m.Brand,
                Model = m.Model,
                Year = m.Year,
                Vin = m.Vin,
                // OwnerName lo lasciamo generico o vuoto, visto che sappiamo già chi è l'utente
                OwnerName = User.Identity?.Name ?? "Mio Garage"
            }).ToList();

            return Ok(dtos);
        }

        // POST: api/Motorcycles (Per aggiungere una moto)
        [HttpPost]
        public async Task<ActionResult<VehicleDto>> PostMotorcycle(VehicleDto vehicleDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var newMoto = new Motorcycle
            {
                Brand = vehicleDto.Brand,
                Model = vehicleDto.Model,
                Year = vehicleDto.Year,
                Vin = vehicleDto.Vin,
                UserId = userId!, // Colleghiamo la moto all'utente loggato
                CreatedAt = DateTime.UtcNow
            };

            _context.Motorcycles.Add(newMoto);
            await _context.SaveChangesAsync();

            vehicleDto.Id = newMoto.Id; // Restituiamo l'ID generato dal DB

            return CreatedAtAction(nameof(GetMotorcycles), new { id = newMoto.Id }, vehicleDto);
        }
    }
}