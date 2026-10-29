using FluentValidation;
using Taxi.DTOs;

namespace Taxi.Validators;

public abstract class CarBaseDtoValidator<T> : AbstractValidator<T> where T : CarBaseDto
{
    protected CarBaseDtoValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        var maxYear = DateTime.UtcNow.Year + 1;

        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Марка автомобіля є обов'язковою.")
            .MaximumLength(50).WithMessage("Марка автомобіля не може перевищувати 50 символів.");

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Модель автомобіля є обов'язковою.")
            .MaximumLength(50).WithMessage("Модель автомобіля не може перевищувати 50 символів.");

        RuleFor(x => x.EngineVolume)
            .InclusiveBetween(0.0, 10.0).WithMessage("Об'єм двигуна має бути від 0 до 10 л (0 — для електромобілів).");

        RuleFor(x => x.Year)
            .InclusiveBetween(ValidationRules.MinCarYear, maxYear).WithMessage($"Рік випуску має бути в межах від {ValidationRules.MinCarYear} до {maxYear}.");
    }
}