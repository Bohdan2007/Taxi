using Microsoft.Extensions.Options;
using Taxi.DTOs;
using Taxi.Options;

namespace Taxi.Validators;

public class CarUpdateDtoValidator : CarBaseDtoValidator<CarUpdateDto>
{
    public CarUpdateDtoValidator(IOptionsMonitor<TaxiFleetOptions> fleetOptions) : base(fleetOptions)
    {
    }
}