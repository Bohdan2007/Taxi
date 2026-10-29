using System.Text.Encodings.Web;
using System.Text.Unicode;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Taxi.Data;
using Taxi.Mappings;
using Taxi.Middleware;
using Taxi.Repositories;
using Taxi.Services;
using Taxi.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers(options =>
    {
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Рядок підключення 'DefaultConnection' не знайдено.");

builder.Services.AddDbContext<TaxiDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddValidatorsFromAssemblyContaining<CarCreateDtoValidator>();
builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();
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
        else
        {
            app.Logger.LogWarning(
                "Міграцій не знайдено. Виконайте 'dotnet ef migrations add InitialCreate' та 'dotnet ef database update'.");
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Не вдалося підключитися до PostgreSQL або застосувати міграції. Перевірте рядок підключення.");
    }
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
