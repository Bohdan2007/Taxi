using AutoMapper;
using FluentValidation;
using Taxi.DTOs;
using Taxi.Exceptions;
using Taxi.Models;
using Taxi.Repositories;

namespace Taxi.Services;

public class CarService : ICarService
{
    private readonly ICarRepository carRepository;
    private readonly IDriverRepository driverRepository;
    private readonly IMapper mapper;
    private readonly IValidator<CarCreateDto> createValidator;
    private readonly IValidator<CarUpdateDto> updateValidator;
    private readonly ILogger<CarService> logger;

    public CarService(ICarRepository carRepository, IDriverRepository driverRepository, IMapper mapper, IValidator<CarCreateDto> createValidator, IValidator<CarUpdateDto> updateValidator, ILogger<CarService> logger)
    {
        this.carRepository = carRepository;
        this.driverRepository = driverRepository;
        this.mapper = mapper;
        this.createValidator = createValidator;
        this.updateValidator = updateValidator;
        this.logger = logger;
    }

    public async Task<List<CarResponseDto>> GetAllAsync()
    {
        logger.LogInformation("Виконано запит на отримання всіх автомобілів.");
        var cars = await carRepository.GetAllAsync();
        return mapper.Map<List<CarResponseDto>>(cars);
    }

    public async Task<CarResponseDto> GetByIdAsync(int id)
    {
        logger.LogInformation("Пошук автомобіля з ID: {Id}", id);
        var car = await GetExistingCarAsync(id);
        logger.LogInformation("Автомобіль знайдено: {Brand} {Model}", car.Brand, car.Model);
        return mapper.Map<CarResponseDto>(car);
    }

    public async Task<CarResponseDto> CreateAsync(CarCreateDto dto)
    {
        await createValidator.ValidateAndThrowAsync(dto);

        var car = mapper.Map<Car>(dto);
        await carRepository.AddAsync(car);

        logger.LogInformation("[Успіх] Додано новий автомобіль: {Brand} {Model}, ID: {Id}", car.Brand, car.Model, car.Id);
        return mapper.Map<CarResponseDto>(car);
    }

    public async Task<CarResponseDto> UpdateAsync(int id, CarUpdateDto dto)
    {
        await updateValidator.ValidateAndThrowAsync(dto);

        var car = await GetExistingCarAsync(id);

        mapper.Map(dto, car);
        await carRepository.UpdateAsync(car);

        logger.LogInformation("[Успіх] Автомобіль ID {Id} успішно оновлено.", id);
        return mapper.Map<CarResponseDto>(car);
    }

    public async Task DeleteAsync(int id)
    {
        var car = await GetExistingCarAsync(id);
        await carRepository.DeleteAsync(car);
        logger.LogInformation("[Успіх] Автомобіль ID {Id} та його водіїв видалено з бази.", id);
    }

    public async Task DeleteAllAsync()
    {
        var deleted = await carRepository.DeleteAllAsync();
        logger.LogWarning("[Увага] Видалено всі автомобілі ({Count}) разом з їхніми водіями!", deleted);
    }

    public async Task<List<DriverResponseDto>> GetDriversByCarIdAsync(int carId)
    {
        if (!await carRepository.ExistsAsync(carId))
        {
            logger.LogWarning("[Увага] Спроба знайти водіїв для неіснуючого авто (ID {Id})", carId);
            throw new NotFoundException($"Автомобіль з ID {carId} не знайдено.");
        }

        var drivers = await driverRepository.GetByCarIdAsync(carId);
        return mapper.Map<List<DriverResponseDto>>(drivers);
    }

    private async Task<Car> GetExistingCarAsync(int id)
    {
        var car = await carRepository.GetByIdAsync(id);
        if (car is null)
        {
            logger.LogWarning("[Увага] Автомобіль з ID {Id} не знайдено в базі!", id);
            throw new NotFoundException($"Автомобіль з ID {id} не знайдено.");
        }

        return car;
    }
}