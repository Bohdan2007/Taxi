using System.Text.Encodings.Web;
using System.Text.Unicode;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Taxi.Caching;
using Taxi.Data;
using Taxi.Mappings;
using Taxi.Middleware;
using Taxi.Options;
using Taxi.Repositories;
using Taxi.Services;
using Taxi.Validators;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

builder.Services
    .AddControllers(mvcOptions =>
    {
        mvcOptions.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    })
    .AddJsonOptions(jsonOptions =>
    {
        jsonOptions.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions<TaxiFleetOptions>().BindConfiguration(TaxiFleetOptions.SectionName).ValidateDataAnnotations().ValidateOnStart();

builder.Services.AddOptions<CachingOptions>().BindConfiguration(CachingOptions.SectionName).ValidateDataAnnotations().ValidateOnStart();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Рядок підключення 'DefaultConnection' не знайдено.");

builder.Services.AddDbContext<TaxiDbContext>(dbOptions => dbOptions.UseNpgsql(connectionString));

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, CacheService>();

builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<IDriverRepository, DriverRepository>();

builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IDriverService, DriverService>();

builder.Services.AddValidatorsFromAssemblyContaining<CarCreateDtoValidator>();
builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.Services.GetRequiredService<IMapper>().ConfigurationProvider.AssertConfigurationIsValid();

    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<TaxiDbContext>();

    try
    {
        if (db.Database.GetMigrations().Any())
        {
            await db.Database.MigrateAsync();
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Не вдалося підключитися до PostgreSQL або застосувати міграції.");
    }
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
