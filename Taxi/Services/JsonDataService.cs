using System.IO;
using System.Text.Json;
using Taxi.Models;

namespace Taxi.Services
{
    public class JsonDataService : IJsonDataService
    {
        private readonly string filePath = "data.json";

        public DataStore LoadData()
        {
            if (!File.Exists(filePath))
            {
                return new DataStore();
            }
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<DataStore>(json) ?? new DataStore();
        }

        public void SaveData(DataStore data)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(filePath, json); 
        }
    }
}