using FluentValidation;
using Taxi.DTOs;

namespace Taxi.Validators;

public abstract class DriverBaseDtoValidator<T> : AbstractValidator<T> where T : DriverBaseDto
{
    protected DriverBaseDtoValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("ПІБ водія є обов'язковим.")
            .MinimumLength(3).WithMessage("ПІБ водія має містити щонайменше 3 символи.")
            .MaximumLength(100).WithMessage("ПІБ водія не може перевищувати 100 символів.")
            .Must(ValidationRules.IsValidPersonName).WithMessage("ПІБ може містити лише літери, пробіли, дефіс та апостроф.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(0.0, 5.0).WithMessage("Рейтинг водія має бути в межах від 0 до 5.");

        RuleFor(x => x.CarId)
            .GreaterThan(0).WithMessage("Ідентифікатор автомобіля (CarId) має бути додатним числом.");
    }
}