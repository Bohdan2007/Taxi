namespace Taxi.Models;

public class Car
{
    public int Id { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public double EngineVolume { get; set; }
    public int Year { get; set; }
    public ICollection<Driver> Drivers { get; set; } = new List<Driver>();
}