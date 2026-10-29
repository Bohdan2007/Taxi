using Taxi.DTOs;

namespace Taxi.Services;

public interface ICarService
{
    Task<List<CarResponseDto>> GetAllAsync();
    Task<CarResponseDto> GetByIdAsync(int id);
    Task<CarResponseDto> CreateAsync(CarCreateDto dto);
    Task<CarResponseDto> UpdateAsync(int id, CarUpdateDto dto);
    Task DeleteAsync(int id);
    Task DeleteAllAsync();
    Task<List<DriverResponseDto>> GetDriversByCarIdAsync(int carId);
}