namespace Taxi.Models;

public class Driver
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public double Rating { get; set; }
    public bool IsBusy { get; set; }
    public int CarId { get; set; }
    public Car? Car { get; set; }
}