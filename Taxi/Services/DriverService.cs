using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Taxi.Exceptions;
using Taxi.Models;

namespace Taxi.Services
{
    public class DriverService : IDriverService
    {
        private readonly IJsonDataService dataService;
        private readonly ILogger<DriverService> logger;

        public DriverService(IJsonDataService dataService, ILogger<DriverService> logger)
        {
            this.dataService = dataService;
            this.logger = logger;
        }

        public List<Driver> GetAll()
        {
            logger.LogInformation("Виконано запит на отримання всіх водіїв.");
            var data = dataService.LoadData();
            return data.Driver;
        }

        public Driver GetById(int id)
        {
            logger.LogInformation($"Пошук водія з ID: {id}");
            var data = dataService.LoadData();
            var driver = data.Driver.FirstOrDefault(d => d.Id == id);
            
            if (driver == null)
            {
                logger.LogWarning($"[Увага] Водія з ID {id} не знайдено в базі!");
                throw new NotFoundException($"Водія з ID {id} не знайдено.");
            }
            
            logger.LogInformation($"Водія знайдено: {driver.FullName}");
            return driver;
        }

        public Driver Add(Driver driver)
        {
            var data = dataService.LoadData();
            
            if (!data.Cars.Any(c => c.Id == driver.CarId))
            {
                logger.LogWarning($"[Помилка] Спроба прив'язати водія до неіснуючого авто (CarId {driver.CarId})");
                throw new BadRequestException($"Неможливо додати водія: автомобіль з ID {driver.CarId} не існує.");
            }

            if (data.Driver.Any())
            {
                driver.Id = data.Driver.Max(d => d.Id) + 1;
            }
            else
            {
                driver.Id = 1;
            }
            
            data.Driver.Add(driver);
            dataService.SaveData(data);
            
            logger.LogInformation($"[Успіх] Додано нового водія: {driver.FullName}, ID: {driver.Id}");
            return driver;
        }

        public Driver Update(int id, Driver updatedDriver)
        {
            var data = dataService.LoadData();
            var driver = data.Driver.FirstOrDefault(d => d.Id == id);
            
            if (driver == null) 
            {
                logger.LogWarning($"[Увага] Спроба оновити неіснуючого водія (ID {id})");
                throw new NotFoundException($"Водія з ID {id} не знайдено.");
            }
            
            if (driver.CarId != updatedDriver.CarId && !data.Cars.Any(c => c.Id == updatedDriver.CarId))
            {
                logger.LogWarning($"[Помилка] Спроба перевести водія на неіснуюче авто (CarId {updatedDriver.CarId})");
                throw new BadRequestException($"Неможливо оновити: автомобіль з ID {updatedDriver.CarId} не існує.");
            }
            
            driver.FullName = updatedDriver.FullName;
            driver.Rating = updatedDriver.Rating;
            driver.IsBusy = updatedDriver.IsBusy;
            driver.CarId = updatedDriver.CarId;

            dataService.SaveData(data);
            logger.LogInformation($"[Успіх] Дані водія ID {id} успішно оновлено.");
            return driver;
        }

        public void Delete(int id)
        {
            var data = dataService.LoadData();
            var driver = data.Driver.FirstOrDefault(d => d.Id == id);
            
            if (driver == null) 
            {
                logger.LogWarning($"[Увага] Спроба видалити неіснуючого водія (ID {id})");
                throw new NotFoundException($"Водія з ID {id} не знайдено.");
            }

            data.Driver.Remove(driver);
            dataService.SaveData(data);
            logger.LogInformation($"[Успіх] Водія ID {id} видалено з бази.");
        }

        public void DeleteAll()
        {
            var data = dataService.LoadData();
            data.Driver.Clear();
            dataService.SaveData(data);
            logger.LogInformation("[Увага] Всі водії були видалені з бази даних!");
        }
    }
}