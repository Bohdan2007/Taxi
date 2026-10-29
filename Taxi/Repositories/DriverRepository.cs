using Microsoft.EntityFrameworkCore;
using Taxi.Data;
using Taxi.Models;

namespace Taxi.Repositories;

public class DriverRepository : IDriverRepository
{
    private readonly TaxiDbContext context;

    public DriverRepository(TaxiDbContext context)
    {
        this.context = context;
    }

    public async Task<List<Driver>> GetAllAsync()
    {
        return await context.Drivers.AsNoTracking().OrderBy(d => d.Id).ToListAsync();
    }

    public async Task<Driver?> GetByIdAsync(int id)
    {
        return await context.Drivers.FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<List<Driver>> GetByCarIdAsync(int carId)
    {
        return await context.Drivers.AsNoTracking().Where(d => d.CarId == carId).OrderBy(d => d.Id).ToListAsync();
    }

    public async Task AddAsync(Driver driver)
    {
        await context.Drivers.AddAsync(driver);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Driver driver)
    {
        context.Drivers.Update(driver);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Driver driver)
    {
        context.Drivers.Remove(driver);
        await context.SaveChangesAsync();
    }

    public async Task<int> DeleteAllAsync()
    {
        return await context.Drivers.ExecuteDeleteAsync();
    }
}