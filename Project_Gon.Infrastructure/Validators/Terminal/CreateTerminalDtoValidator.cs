namespace Project_Gon.Infrastructure.Validators.Terminal;

using FluentValidation;
using Project_Gon.Core.DTOs.Terminal;

public class CreateTerminalDtoValidator : AbstractValidator<CreateTerminalDto>
{
    public CreateTerminalDtoValidator()
    {
        RuleFor(x => x.SucursalId)
            .GreaterThan(0)
            .WithMessage("El ID de la sucursal es requerido.");

        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre del terminal es requerido.")
            .MaximumLength(100)
            .WithMessage("El nombre no puede exceder 100 caracteres.");

        RuleFor(x => x.NumeroSerie)
            .MaximumLength(50)
            .WithMessage("El número de serie no puede exceder 50 caracteres.");
    }
}