using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Taxi.Exceptions;
using Taxi.Models;

namespace Taxi.Services
{
    public class CarService : ICarService
    {
        private readonly IJsonDataService dataService;
        private readonly ILogger<CarService> logger; 

        public CarService(IJsonDataService dataService, ILogger<CarService> logger)
        {
            this.dataService = dataService;
            this.logger = logger;
        }

        public List<Car> GetAll()
        {
            logger.LogInformation("Виконано запит на отримання всіх автомобілів.");
            var data = dataService.LoadData();
            return data.Cars;
        }

        public Car GetById(int id)
        {
            logger.LogInformation($"Пошук автомобіля з ID: {id}");
            var data = dataService.LoadData();
            var car = data.Cars.FirstOrDefault(c => c.Id == id);
            
            if (car == null)
            {
                logger.LogWarning($"[Увага] Автомобіль з ID {id} не знайдено в базі!");
                throw new NotFoundException($"Автомобіль з ID {id} не знайдено.");
            }
            
            logger.LogInformation($"Автомобіль знайдено: {car.Brand} {car.Model}");
            return car;
        }

        public Car Add(Car car)
        {
            var data = dataService.LoadData();
            
            if (data.Cars.Any())
            {
                car.Id = data.Cars.Max(c => c.Id) + 1;
            }
            else
            {
                car.Id = 1;
            }
            
            data.Cars.Add(car);
            dataService.SaveData(data);
            
            logger.LogInformation($"[Успіх] Додано новий автомобіль: {car.Brand} {car.Model}, ID: {car.Id}");
            return car;
        }

        public Car Update(int id, Car updatedCar)
        {
            var data = dataService.LoadData();
            var car = data.Cars.FirstOrDefault(c => c.Id == id);
            
            if (car == null) 
            {
                logger.LogWarning($"[Увага] Спроба оновити неіснуючий автомобіль (ID {id})");
                throw new NotFoundException($"Автомобіль з ID {id} не знайдено.");
            }

            car.Brand = updatedCar.Brand;
            car.Model = updatedCar.Model;
            car.EngineVolume = updatedCar.EngineVolume;
            car.Year = updatedCar.Year;

            dataService.SaveData(data);
            logger.LogInformation($"[Успіх] Автомобіль ID {id} успішно оновлено.");
            return car;
        }

        public void Delete(int id)
        {
            var data = dataService.LoadData();
            var car = data.Cars.FirstOrDefault(c => c.Id == id);
            
            if (car == null) 
            {
                logger.LogWarning($"[Увага] Спроба видалити неіснуючий автомобіль (ID {id})");
                throw new NotFoundException($"Автомобіль з ID {id} не знайдено.");
            }

            data.Cars.Remove(car);
            data.Driver.RemoveAll(d => d.CarId == id);
            
            dataService.SaveData(data);
            logger.LogInformation($"[Успіх] Автомобіль ID {id} та його водіїв видалено з бази.");
        }

        public void DeleteAll()
        {
            var data = dataService.LoadData();
            data.Cars.Clear();
            data.Driver.Clear();
            dataService.SaveData(data);
            logger.LogInformation("[Увага] Всі автомобілі та водії були видалені з бази даних!");
        }

        public List<Driver> GetDriversByCarId(int carId)
        {
            var data = dataService.LoadData();
            
            if (!data.Cars.Any(c => c.Id == carId))
            {
                logger.LogWarning($"[Увага] Спроба знайти водіїв для неіснуючого авто (ID {carId})");
                throw new NotFoundException($"Автомобіль з ID {carId} не знайдено.");
            }

            return data.Driver.Where(d => d.CarId == carId).ToList();
        }
    }
}