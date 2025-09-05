using Mottu.Api.Dtos;
using Mottu.Api.Entities;

namespace Mottu.Api.Services
{
    public interface IMotorcycleService
    {
        Task<Motorcycle> CreateMotorcycleAsync(CreateMotorcycleDto createMotorcycleDto);
        Task<Motorcycle?> GetMotorcycleByIdAsync(Guid id);
        Task<IEnumerable<Motorcycle>> ListMotorcyclesAsync(string? plate = null);
        Task<bool> UpdateMotorcyclePlateByIdAsync(Guid id, UpdateMotorcyclePlateDto updateDto);
        Task<bool> DeleteMotorcycleAsync(Guid id);
        Task<bool> IsPlateUniqueAsync(string plate, Guid? excludeId = null);
    }
}
