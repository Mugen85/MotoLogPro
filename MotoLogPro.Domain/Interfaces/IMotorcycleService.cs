using MotoLogPro.Shared.DTOs;

namespace MotoLogPro.Domain.Interfaces
{
    public interface IMotorcycleService
    {
        Task<IEnumerable<VehicleDto>> GetByUserAsync(string userId);
        Task<VehicleDto> CreateAsync(string userId, CreateMotorcycleDto dto);
        Task<bool> UpdateAsync(string userId, int id, CreateMotorcycleDto dto);
        Task<bool> DeleteAsync(string userId, int id);
    }
}