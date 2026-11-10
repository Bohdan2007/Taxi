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

public class CarService : ICarService
{
    private readonly ICarRepository carRepository;
    private readonly IDriverRepository driverRepository;
    private readonly IMapper mapper;
    private readonly IValidator<CarCreateDto> createValidator;
    private readonly IValidator<CarUpdateDto> updateValidator;
    private readonly ICacheService cache;
    private readonly IOptionsMonitor<TaxiFleetOptions> fleetOptions;
    private readonly ILogger<CarService> logger;

    public CarService(
        ICarRepository carRepository,
        IDriverRepository driverRepository,
        IMapper mapper,
        IValidator<CarCreateDto> createValidator,
        IValidator<CarUpdateDto> updateValidator,
        ICacheService cache,
        IOptionsMonitor<TaxiFleetOptions> fleetOptions,
        ILogger<CarService> logger)
    {
        this.carRepository = carRepository;
        this.driverRepository = driverRepository;
        this.mapper = mapper;
        this.createValidator = createValidator;
        this.updateValidator = updateValidator;
        this.cache = cache;
        this.fleetOptions = fleetOptions;
        this.logger = logger;
    }

    public async Task<List<CarResponseDto>> GetAllAsync()
    {
        logger.LogInformation(LogEvents.CarsGetAll, "Запит на отримання всіх автомобілів");

        return await cache.GetOrCreateAsync(CacheKeys.AllCars, async () =>
        {
            var cars = await carRepository.GetAllAsync();
            return mapper.Map<List<CarResponseDto>>(cars);
        });
    }

    public async Task<CarResponseDto> GetByIdAsync(int id)
    {
        logger.LogInformation(LogEvents.CarGetById, "Запит на отримання автомобіля ID {Id}", id);

        return await cache.GetOrCreateAsync(CacheKeys.Car(id), async () =>
        {
            var car = await GetExistingCarAsync(id);
            return mapper.Map<CarResponseDto>(car);
        });
    }

    public async Task<CarResponseDto> CreateAsync(CarCreateDto dto)
    {
        await createValidator.ValidateAndThrowAsync(dto);

        var car = mapper.Map<Car>(dto);
        await carRepository.AddAsync(car);

        cache.Remove(CacheKeys.AllCars);

        logger.LogInformation(LogEvents.CarCreated, "[Успіх] Додано автомобіль {Brand} {Model}, ID {Id}",
            car.Brand, car.Model, car.Id);
        return mapper.Map<CarResponseDto>(car);
    }

    public async Task<CarResponseDto> UpdateAsync(int id, CarUpdateDto dto)
    {
        await updateValidator.ValidateAndThrowAsync(dto);

        var car = await GetExistingCarAsync(id);
        mapper.Map(dto, car);
        await carRepository.UpdateAsync(car);

        cache.Remove(CacheKeys.AllCars, CacheKeys.Car(id));

        logger.LogInformation(LogEvents.CarUpdated, "[Успіх] Оновлено автомобіль ID {Id}", id);
        return mapper.Map<CarResponseDto>(car);
    }

    public async Task DeleteAsync(int id)
    {
        var car = await GetExistingCarAsync(id);
        await carRepository.DeleteAsync(car);
        
        cache.Clear();
        
        logger.LogInformation(LogEvents.CarDeleted, "[Успіх] Видалено автомобіль ID {Id} разом з його водіями", id);
    }

    public async Task DeleteAllAsync()
    {
        if (!fleetOptions.CurrentValue.AllowDeleteAll)
        {
            logger.LogWarning(LogEvents.Forbidden, "[Заборонено] Спроба масового видалення автомобілів при TaxiFleet:AllowDeleteAll = false");
            throw new ForbiddenException("Масове видалення автомобілів заборонено налаштуваннями сервера.");
        }

        var deleted = await carRepository.DeleteAllAsync();
        cache.Clear();

        logger.LogWarning(LogEvents.CarsDeletedAll, "[Увага] Видалено всі автомобілі ({Count}) разом з їхніми водіями", deleted);
    }

    public async Task<List<DriverResponseDto>> GetDriversByCarIdAsync(int carId)
    {
        logger.LogInformation(LogEvents.CarDriversGet, "Запит на отримання водіїв автомобіля ID {CarId}", carId);

        return await cache.GetOrCreateAsync(CacheKeys.CarDrivers(carId), async () =>
        {
            if (!await carRepository.ExistsAsync(carId))
            {
                logger.LogWarning(LogEvents.CarNotFound, "Автомобіль ID {Id} не знайдено", carId);
                throw new NotFoundException($"Автомобіль з ID {carId} не знайдено.");
            }

            var drivers = await driverRepository.GetByCarIdAsync(carId);
            return mapper.Map<List<DriverResponseDto>>(drivers);
        });
    }

    private async Task<Car> GetExistingCarAsync(int id)
    {
        var car = await carRepository.GetByIdAsync(id);
        if (car is null)
        {
            logger.LogWarning(LogEvents.CarNotFound, "Автомобіль ID {Id} не знайдено", id);
            throw new NotFoundException($"Автомобіль з ID {id} не знайдено.");
        }

        return car;
    }
}
