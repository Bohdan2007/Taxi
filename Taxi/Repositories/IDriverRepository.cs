using Taxi.Models;

namespace Taxi.Repositories;

public interface IDriverRepository
{
    Task<List<Driver>> GetAllAsync();
    Task<Driver?> GetByIdAsync(int id);
    Task<List<Driver>> GetByCarIdAsync(int carId);
    Task AddAsync(Driver driver);
    Task UpdateAsync(Driver driver);
    Task DeleteAsync(Driver driver);
    Task<int> DeleteAllAsync();
}