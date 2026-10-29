namespace Taxi.DTOs;

public abstract class DriverBaseDto
{
    public string FullName { get; set; } = string.Empty;
    public double Rating { get; set; }
    public bool IsBusy { get; set; }
    public int CarId { get; set; }
}