using System.ComponentModel.DataAnnotations;

namespace Taxi.Options;

public class TaxiFleetOptions
{
    public const string SectionName = "TaxiFleet";
    
    [Range(1, 10, ErrorMessage = "TaxiFleet:MaxDriversPerCar має бути від 1 до 10.")]
    public int MaxDriversPerCar { get; set; } = 3;
    
    [Range(1, 50, ErrorMessage = "TaxiFleet:MaxCarAgeYears має бути від 1 до 50.")]
    public int MaxCarAgeYears { get; set; } = 15;
    
    public bool AllowDeleteAll { get; set; }
}