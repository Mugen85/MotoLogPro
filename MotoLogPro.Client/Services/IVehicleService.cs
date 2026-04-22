using MotoLogPro.Shared.DTOs;

namespace MotoLogPro.Client.Services
{
    public interface IVehicleService
    {
        // Lettura
        Task<IEnumerable<VehicleDto>> GetVehiclesAsync();

        // Creazione
        Task<VehicleDto?> CreateVehicleAsync(CreateMotorcycleDto dto);

        // Modifica
        Task<bool> UpdateVehicleAsync(int id, CreateMotorcycleDto dto);

        // Cancellazione
        Task<bool> DeleteVehicleAsync(int id);
    }
}