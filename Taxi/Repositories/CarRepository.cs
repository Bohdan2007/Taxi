using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Taxi.Data;
using Taxi.Logging;
using Taxi.Models;

namespace Taxi.Repositories;

public class CarRepository : ICarRepository
{
    private readonly TaxiDbContext context;
    private readonly ILogger<CarRepository> logger;

    public CarRepository(TaxiDbContext context, ILogger<CarRepository> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public async Task<List<Car>> GetAllAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        var cars = await context.Cars.AsNoTracking().OrderBy(c => c.Id).ToListAsync();
        logger.LogDebug(LogEvents.DbRead, "[БД] Отримано автомобілів: {Count} ({Elapsed} мс)", cars.Count, stopwatch.ElapsedMilliseconds);
        return cars;
    }

    public async Task<Car?> GetByIdAsync(int id)
    {
        var stopwatch = Stopwatch.StartNew();
        var car = await context.Cars.FirstOrDefaultAsync(c => c.Id == id);
        logger.LogDebug(LogEvents.DbRead, "[БД] Пошук автомобіля ID {Id}: {Result} ({Elapsed} мс)", id, car is null ? "не знайдено" : "знайдено", stopwatch.ElapsedMilliseconds);
        return car;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        var stopwatch = Stopwatch.StartNew();
        var exists = await context.Cars.AnyAsync(c => c.Id == id);
        logger.LogDebug(LogEvents.DbRead, "[БД] Перевірка існування автомобіля ID {Id}: {Result} ({Elapsed} мс)", id, exists ? "існує" : "не існує", stopwatch.ElapsedMilliseconds);
        return exists;
    }

    public async Task AddAsync(Car car)
    {
        var stopwatch = Stopwatch.StartNew();
        await context.Cars.AddAsync(car);
        await context.SaveChangesAsync();
        logger.LogDebug(LogEvents.DbWrite, "[БД] Додано автомобіль ID {Id} ({Elapsed} мс)", car.Id, stopwatch.ElapsedMilliseconds);
    }

    public async Task UpdateAsync(Car car)
    {
        var stopwatch = Stopwatch.StartNew();
        context.Cars.Update(car);
        await context.SaveChangesAsync();
        logger.LogDebug(LogEvents.DbWrite, "[БД] Оновлено автомобіль ID {Id} ({Elapsed} мс)", car.Id, stopwatch.ElapsedMilliseconds);
    }

    public async Task DeleteAsync(Car car)
    {
        var stopwatch = Stopwatch.StartNew();
        context.Cars.Remove(car);
        await context.SaveChangesAsync();
        logger.LogDebug(LogEvents.DbDelete, "[БД] Видалено автомобіль ID {Id} ({Elapsed} мс)", car.Id, stopwatch.ElapsedMilliseconds);
    }

    public async Task<int> DeleteAllAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        var deleted = await context.Cars.ExecuteDeleteAsync();
        logger.LogDebug(LogEvents.DbDelete, "[БД] Видалено всі автомобілі: {Count} ({Elapsed} мс)", deleted, stopwatch.ElapsedMilliseconds);
        return deleted;
    }
}
