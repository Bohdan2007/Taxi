using Microsoft.EntityFrameworkCore;
using Taxi.Data;
using Taxi.Models;

namespace Taxi.Repositories;

public class CarRepository : ICarRepository
{
    private readonly TaxiDbContext context;

    public CarRepository(TaxiDbContext context)
    {
        this.context = context;
    }

    public async Task<List<Car>> GetAllAsync()
    {
        return await context.Cars.AsNoTracking().OrderBy(c => c.Id).ToListAsync();
    }

    public async Task<Car?> GetByIdAsync(int id)
    {
        return await context.Cars.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Cars.AnyAsync(c => c.Id == id);
    }

    public async Task AddAsync(Car car)
    {
        await context.Cars.AddAsync(car);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Car car)
    {
        context.Cars.Update(car);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Car car)
    {
        context.Cars.Remove(car);
        await context.SaveChangesAsync();
    }

    public async Task<int> DeleteAllAsync()
    {
        return await context.Cars.ExecuteDeleteAsync();
    }
}