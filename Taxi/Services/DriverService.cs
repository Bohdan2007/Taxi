using AutoMapper;
using FluentValidation;
using Taxi.DTOs;
using Taxi.Exceptions;
using Taxi.Models;
using Taxi.Repositories;

namespace Taxi.Services;

public class DriverService : IDriverService
{
    private readonly IDriverRepository driverRepository;
    private readonly ICarRepository carRepository;
    private readonly IMapper mapper;
    private readonly IValidator<DriverCreateDto> createValidator;
    private readonly IValidator<DriverUpdateDto> updateValidator;
    private readonly ILogger<DriverService> logger;

    public DriverService(IDriverRepository driverRepository, ICarRepository carRepository, IMapper mapper, IValidator<DriverCreateDto> createValidator, IValidator<DriverUpdateDto> updateValidator, ILogger<DriverService> logger)
    {
        this.driverRepository = driverRepository;
        this.carRepository = carRepository;
        this.mapper = mapper;
        this.createValidator = createValidator;
        this.updateValidator = updateValidator;
        this.logger = logger;
    }

    public async Task<List<DriverResponseDto>> GetAllAsync()
    {
        logger.LogInformation("Виконано запит на отримання всіх водіїв.");
        var drivers = await driverRepository.GetAllAsync();
        return mapper.Map<List<DriverResponseDto>>(drivers);
    }

    public async Task<DriverResponseDto> GetByIdAsync(int id)
    {
        logger.LogInformation("Пошук водія з ID: {Id}", id);
        var driver = await GetExistingDriverAsync(id);
        logger.LogInformation("Водія знайдено: {FullName}", driver.FullName);
        return mapper.Map<DriverResponseDto>(driver);
    }

    public async Task<DriverResponseDto> CreateAsync(DriverCreateDto dto)
    {
        await createValidator.ValidateAndThrowAsync(dto);
        await EnsureCarExistsAsync(dto.CarId);

        var driver = mapper.Map<Driver>(dto);
        await driverRepository.AddAsync(driver);

        logger.LogInformation("[Успіх] Додано нового водія: {FullName}, ID: {Id}", driver.FullName, driver.Id);
        return mapper.Map<DriverResponseDto>(driver);
    }

    public async Task<DriverResponseDto> UpdateAsync(int id, DriverUpdateDto dto)
    {
        await updateValidator.ValidateAndThrowAsync(dto);

        var driver = await GetExistingDriverAsync(id);

        if (driver.CarId != dto.CarId)
        {
            await EnsureCarExistsAsync(dto.CarId);
        }

        mapper.Map(dto, driver);
        await driverRepository.UpdateAsync(driver);

        logger.LogInformation("[Успіх] Дані водія ID {Id} успішно оновлено.", id);
        return mapper.Map<DriverResponseDto>(driver);
    }

    public async Task DeleteAsync(int id)
    {
        var driver = await GetExistingDriverAsync(id);
        await driverRepository.DeleteAsync(driver);
        logger.LogInformation("[Успіх] Водія ID {Id} видалено з бази.", id);
    }

    public async Task DeleteAllAsync()
    {
        var deleted = await driverRepository.DeleteAllAsync();
        logger.LogWarning("[Увага] Видалено всіх водіїв ({Count})!", deleted);
    }

    private async Task<Driver> GetExistingDriverAsync(int id)
    {
        var driver = await driverRepository.GetByIdAsync(id);
        if (driver is null)
        {
            logger.LogWarning("[Увага] Водія з ID {Id} не знайдено в базі!", id);
            throw new NotFoundException($"Водія з ID {id} не знайдено.");
        }

        return driver;
    }

    private async Task EnsureCarExistsAsync(int carId)
    {
        if (!await carRepository.ExistsAsync(carId))
        {
            logger.LogWarning("[Помилка] Спроба прив'язати водія до неіснуючого авто (CarId {CarId})", carId);
            throw new BadRequestException($"Автомобіль з ID {carId} не існує.");
        }
    }
}