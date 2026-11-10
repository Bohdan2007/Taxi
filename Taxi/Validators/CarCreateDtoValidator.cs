using Microsoft.Extensions.Options;
using Taxi.DTOs;
using Taxi.Options;

namespace Taxi.Validators;

public class CarCreateDtoValidator : CarBaseDtoValidator<CarCreateDto>
{
    public CarCreateDtoValidator(IOptionsMonitor<TaxiFleetOptions> fleetOptions) : base(fleetOptions)
    {
    }
}