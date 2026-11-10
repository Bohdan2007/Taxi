namespace Taxi.Caching;

public static class CacheKeys
{
    public const string AllCars = "cars:all";
    public const string AllDrivers = "drivers:all";

    public static string Car(int id) => $"cars:{id}";
    public static string CarDrivers(int carId) => $"cars:{carId}:drivers";
    public static string Driver(int id) => $"drivers:{id}";
}