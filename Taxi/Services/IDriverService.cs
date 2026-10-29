using Taxi.DTOs;

namespace Taxi.Services;

public interface IDriverService
{
    Task<List<DriverResponseDto>> GetAllAsync();
    Task<DriverResponseDto> GetByIdAsync(int id);
    Task<DriverResponseDto> CreateAsync(DriverCreateDto dto);
    Task<DriverResponseDto> UpdateAsync(int id, DriverUpdateDto dto);
    Task DeleteAsync(int id);
    Task DeleteAllAsync();
}