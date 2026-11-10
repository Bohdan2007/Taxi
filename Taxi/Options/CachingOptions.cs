using System.ComponentModel.DataAnnotations;

namespace Taxi.Options;

public class CachingOptions
{
    public const string SectionName = "Caching";
    
    public bool Enabled { get; set; } = true;
    
    [Range(1, 3600, ErrorMessage = "Caching:DurationSeconds має бути від 1 до 3600.")]
    public int DurationSeconds { get; set; } = 60;
}