namespace Taxi.Logging;

public static class LogEvents
{
    public const int CarsGetAll = 1001;
    public const int CarGetById = 1002;
    public const int CarCreated = 1003;
    public const int CarUpdated = 1004;
    public const int CarDeleted = 1005;
    public const int CarsDeletedAll = 1006;
    public const int CarDriversGet = 1007;
    public const int CarNotFound = 1404;
    
    public const int DriversGetAll = 2001;
    public const int DriverGetById = 2002;
    public const int DriverCreated = 2003;
    public const int DriverUpdated = 2004;
    public const int DriverDeleted = 2005;
    public const int DriversDeletedAll = 2006;
    public const int DriverCarMissing = 2400;
    public const int DriverNotFound = 2404;
    public const int DriverLimitReached = 2409;
    
    public const int DbRead = 3001;
    public const int DbWrite = 3002;
    public const int DbDelete = 3003;
    
    public const int CacheHit = 4001;
    public const int CacheMiss = 4002;
    public const int CacheStored = 4003;
    public const int CacheRemoved = 4004;
    public const int CacheCleared = 4005;
    public const int CacheDisabled = 4006;
    
    public const int ValidationFailed = 5400;
    public const int Forbidden = 5403;
    public const int UnhandledError = 5500;
}