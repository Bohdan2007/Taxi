using FluentValidation;
using Microsoft.Extensions.Options;
using Taxi.DTOs;
using Taxi.Options;

namespace Taxi.Validators;

public abstract class CarBaseDtoValidator<T> : AbstractValidator<T> where T : CarBaseDto
{
    protected CarBaseDtoValidator(IOptionsMonitor<TaxiFleetOptions> fleetOptions)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        var currentYear = DateTime.UtcNow.Year;
        var maxCarAge = fleetOptions.CurrentValue.MaxCarAgeYears;
        var minYear = currentYear - maxCarAge;
        var maxYear = currentYear + 1;

        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Марка автомобіля є обов'язковою.")
            .MaximumLength(50).WithMessage("Марка автомобіля не може перевищувати 50 символів.");

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Модель автомобіля є обов'язковою.")
            .MaximumLength(50).WithMessage("Модель автомобіля не може перевищувати 50 символів.");

        RuleFor(x => x.EngineVolume)
            .InclusiveBetween(0.0, 10.0).WithMessage("Об'єм двигуна має бути від 0 до 10 л (0 — для електромобілів).");

        RuleFor(x => x.Year)
            .InclusiveBetween(minYear, maxYear)
            .WithMessage($"Рік випуску має бути від {minYear} до {maxYear}: до роботи в таксопарку допускаються автомобілі не старші за {maxCarAge} років.");
    }
}