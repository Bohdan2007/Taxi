using System.Collections.Generic;

namespace Taxi.Models
{
    public class DataStore
    {
        public List<Car> Cars { get; set; } = new List<Car>();
        public List<Driver> Driver { get; set; } = new List<Driver>();
    }
}