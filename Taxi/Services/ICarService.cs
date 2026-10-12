using System.Collections.Generic;
using Taxi.Models;

namespace Taxi.Services
{
    public interface ICarService
    {
        List<Car> GetAll();
        Car GetById(int id);
        Car Add(Car car);
        Car Update(int id, Car car);
        void Delete(int id);
        void DeleteAll();
        List<Driver> GetDriversByCarId(int carId);
    }
}