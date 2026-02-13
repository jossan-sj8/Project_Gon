namespace Project_Gon.Infrastructure.Validators.Terminal;

using FluentValidation;
using Project_Gon.Core.DTOs.Terminal;

public class UpdateTerminalDtoValidator : AbstractValidator<UpdateTerminalDto>
{
    public UpdateTerminalDtoValidator()
    {
        RuleFor(x => x.Nombre)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Nombre))
            .WithMessage("El nombre no puede exceder 100 caracteres.");

        RuleFor(x => x.NumeroSerie)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.NumeroSerie))
            .WithMessage("El número de serie no puede exceder 50 caracteres.");
    }
}