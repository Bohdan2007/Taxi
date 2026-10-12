using System.Threading.Tasks;
using Taxi.Models;

namespace Taxi.Services
{
    public interface IJsonDataService
    {
        DataStore LoadData();
        void SaveData(DataStore data);
    }
}