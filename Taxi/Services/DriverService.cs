using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Options;
using Taxi.Caching;
using Taxi.DTOs;
using Taxi.Exceptions;
using Taxi.Logging;
using Taxi.Models;
using Taxi.Options;
using Taxi.Repositories;

namespace Taxi.Services;

public class DriverService : IDriverService
{
    private readonly IDriverRepository driverRepository;
    private readonly ICarRepository carRepository;
    private readonly IMapper mapper;
    private readonly IValidator<DriverCreateDto> createValidator;
    private readonly IValidator<DriverUpdateDto> updateValidator;
    private readonly ICacheService cache;
    private readonly IOptionsMonitor<TaxiFleetOptions> fleetOptions;
    private readonly ILogger<DriverService> logger;

    public DriverService(
        IDriverRepository driverRepository,
        ICarRepository carRepository,
        IMapper mapper,
        IValidator<DriverCreateDto> createValidator,
        IValidator<DriverUpdateDto> updateValidator,
        ICacheService cache,
        IOptionsMonitor<TaxiFleetOptions> fleetOptions,
        ILogger<DriverService> logger)
    {
        this.driverRepository = driverRepository;
        this.carRepository = carRepository;
        this.mapper = mapper;
        this.createValidator = createValidator;
        this.updateValidator = updateValidator;
        this.cache = cache;
        this.fleetOptions = fleetOptions;
        this.logger = logger;
    }

    public async Task<List<DriverResponseDto>> GetAllAsync()
    {
        logger.LogInformation(LogEvents.DriversGetAll, "Запит на отримання всіх водіїв");

        return await cache.GetOrCreateAsync(CacheKeys.AllDrivers, async () =>
        {
            var drivers = await driverRepository.GetAllAsync();
            return mapper.Map<List<DriverResponseDto>>(drivers);
        });
    }

    public async Task<DriverResponseDto> GetByIdAsync(int id)
    {
        logger.LogInformation(LogEvents.DriverGetById, "Запит на отримання водія ID {Id}", id);

        return await cache.GetOrCreateAsync(CacheKeys.Driver(id), async () =>
        {
            var driver = await GetExistingDriverAsync(id);
            return mapper.Map<DriverResponseDto>(driver);
        });
    }

    public async Task<DriverResponseDto> CreateAsync(DriverCreateDto dto)
    {
        await createValidator.ValidateAndThrowAsync(dto);
        await EnsureCarExistsAsync(dto.CarId);
        await EnsureCarHasFreeSlotAsync(dto.CarId);

        var driver = mapper.Map<Driver>(dto);
        await driverRepository.AddAsync(driver);

        cache.Remove(CacheKeys.AllDrivers, CacheKeys.CarDrivers(driver.CarId));

        logger.LogInformation(LogEvents.DriverCreated, "[Успіх] Додано водія {FullName}, ID {Id}", driver.FullName, driver.Id);
        return mapper.Map<DriverResponseDto>(driver);
    }

    public async Task<DriverResponseDto> UpdateAsync(int id, DriverUpdateDto dto)
    {
        await updateValidator.ValidateAndThrowAsync(dto);

        var driver = await GetExistingDriverAsync(id);
        var oldCarId = driver.CarId;

        if (oldCarId != dto.CarId)
        {
            await EnsureCarExistsAsync(dto.CarId);
            await EnsureCarHasFreeSlotAsync(dto.CarId);
        }

        mapper.Map(dto, driver);
        await driverRepository.UpdateAsync(driver);

        cache.Remove(CacheKeys.AllDrivers, CacheKeys.Driver(id), CacheKeys.CarDrivers(oldCarId), CacheKeys.CarDrivers(driver.CarId));

        logger.LogInformation(LogEvents.DriverUpdated, "[Успіх] Оновлено водія ID {Id}", id);
        return mapper.Map<DriverResponseDto>(driver);
    }

    public async Task DeleteAsync(int id)
    {
        var driver = await GetExistingDriverAsync(id);
        var carId = driver.CarId;

        await driverRepository.DeleteAsync(driver);

        cache.Remove(CacheKeys.AllDrivers, CacheKeys.Driver(id), CacheKeys.CarDrivers(carId));

        logger.LogInformation(LogEvents.DriverDeleted, "[Успіх] Видалено водія ID {Id}", id);
    }

    public async Task DeleteAllAsync()
    {
        if (!fleetOptions.CurrentValue.AllowDeleteAll)
        {
            logger.LogWarning(LogEvents.Forbidden, "[Заборонено] Спроба масового видалення водіїв при TaxiFleet:AllowDeleteAll = false");
            throw new ForbiddenException("Масове видалення водіїв заборонено налаштуваннями сервера.");
        }

        var deleted = await driverRepository.DeleteAllAsync();
        cache.Clear();

        logger.LogWarning(LogEvents.DriversDeletedAll, "[Увага] Видалено всіх водіїв ({Count})", deleted);
    }

    private async Task<Driver> GetExistingDriverAsync(int id)
    {
        var driver = await driverRepository.GetByIdAsync(id);
        if (driver is null)
        {
            logger.LogWarning(LogEvents.DriverNotFound, "Водія ID {Id} не знайдено", id);
            throw new NotFoundException($"Водія з ID {id} не знайдено.");
        }

        return driver;
    }

    private async Task EnsureCarExistsAsync(int carId)
    {
        if (!await carRepository.ExistsAsync(carId))
        {
            logger.LogWarning(LogEvents.DriverCarMissing, "Спроба прив'язати водія до неіснуючого автомобіля ID {CarId}", carId);
            throw new BadRequestException($"Автомобіль з ID {carId} не існує.");
        }
    }
    
    private async Task EnsureCarHasFreeSlotAsync(int carId)
    {
        var maxDrivers = fleetOptions.CurrentValue.MaxDriversPerCar;
        var currentCount = await driverRepository.CountByCarIdAsync(carId);

        if (currentCount >= maxDrivers)
        {
            logger.LogWarning(LogEvents.DriverLimitReached, "[Ліміт] За автомобілем ID {CarId} вже {Count} водіїв (максимум {Max})", carId, currentCount, maxDrivers);
            throw new ConflictException($"За автомобілем з ID {carId} вже закріплено максимальну кількість водіїв ({maxDrivers}).");
        }
    }
}
