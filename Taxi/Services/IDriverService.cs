using System.Collections.Generic;
using System.Threading.Tasks;
using Taxi.Models;

namespace Taxi.Services
{
    public interface IDriverService
    {
        List<Driver> GetAll();
        Driver GetById(int id);
        Driver Add(Driver driver);
        Driver Update(int id, Driver driver);
        void Delete(int id);
        void DeleteAll();
    }
}