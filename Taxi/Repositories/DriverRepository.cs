using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Taxi.Data;
using Taxi.Logging;
using Taxi.Models;

namespace Taxi.Repositories;

public class DriverRepository : IDriverRepository
{
    private readonly TaxiDbContext context;
    private readonly ILogger<DriverRepository> logger;

    public DriverRepository(TaxiDbContext context, ILogger<DriverRepository> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public async Task<List<Driver>> GetAllAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        var drivers = await context.Drivers.AsNoTracking().OrderBy(d => d.Id).ToListAsync();
        logger.LogDebug(LogEvents.DbRead, "[БД] Отримано водіїв: {Count} ({Elapsed} мс)", drivers.Count, stopwatch.ElapsedMilliseconds);
        return drivers;
    }

    public async Task<Driver?> GetByIdAsync(int id)
    {
        var stopwatch = Stopwatch.StartNew();
        var driver = await context.Drivers.FirstOrDefaultAsync(d => d.Id == id);
        logger.LogDebug(LogEvents.DbRead, "[БД] Пошук водія ID {Id}: {Result} ({Elapsed} мс)", id, driver is null ? "не знайдено" : "знайдено", stopwatch.ElapsedMilliseconds);
        return driver;
    }

    public async Task<List<Driver>> GetByCarIdAsync(int carId)
    {
        var stopwatch = Stopwatch.StartNew();
        var drivers = await context.Drivers.AsNoTracking().Where(d => d.CarId == carId).OrderBy(d => d.Id).ToListAsync();
        logger.LogDebug(LogEvents.DbRead, "[БД] Отримано водіїв автомобіля ID {CarId}: {Count} ({Elapsed} мс)", carId, drivers.Count, stopwatch.ElapsedMilliseconds);
        return drivers;
    }

    public async Task<int> CountByCarIdAsync(int carId)
    {
        var stopwatch = Stopwatch.StartNew();
        var count = await context.Drivers.CountAsync(d => d.CarId == carId);
        logger.LogDebug(LogEvents.DbRead, "[БД] Кількість водіїв автомобіля ID {CarId}: {Count} ({Elapsed} мс)", carId, count, stopwatch.ElapsedMilliseconds);
        return count;
    }

    public async Task AddAsync(Driver driver)
    {
        var stopwatch = Stopwatch.StartNew();
        await context.Drivers.AddAsync(driver);
        await context.SaveChangesAsync();
        logger.LogDebug(LogEvents.DbWrite, "[БД] Додано водія ID {Id} ({Elapsed} мс)", driver.Id, stopwatch.ElapsedMilliseconds);
    }

    public async Task UpdateAsync(Driver driver)
    {
        var stopwatch = Stopwatch.StartNew();
        context.Drivers.Update(driver);
        await context.SaveChangesAsync();
        logger.LogDebug(LogEvents.DbWrite, "[БД] Оновлено водія ID {Id} ({Elapsed} мс)", driver.Id, stopwatch.ElapsedMilliseconds);
    }

    public async Task DeleteAsync(Driver driver)
    {
        var stopwatch = Stopwatch.StartNew();
        context.Drivers.Remove(driver);
        await context.SaveChangesAsync();
        logger.LogDebug(LogEvents.DbDelete, "[БД] Видалено водія ID {Id} ({Elapsed} мс)", driver.Id, stopwatch.ElapsedMilliseconds);
    }

    public async Task<int> DeleteAllAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        var deleted = await context.Drivers.ExecuteDeleteAsync();
        logger.LogDebug(LogEvents.DbDelete, "[БД] Видалено всіх водіїв: {Count} ({Elapsed} мс)", deleted, stopwatch.ElapsedMilliseconds);
        return deleted;
    }
}
