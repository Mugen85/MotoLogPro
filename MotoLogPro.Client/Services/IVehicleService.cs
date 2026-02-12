using MotoLogPro.Shared.DTOs;

namespace MotoLogPro.Client.Services
{
    public interface IVehicleService
    {
        Task<List<VehicleDto>> GetVehiclesAsync();
    }
}