using FluentValidation;
using Project_Gon.Core.DTOs.MetodoPago;

namespace Project_Gon.Infrastructure.Validators.MetodoPago;

public class CreateMetodoPagoDtoValidator : AbstractValidator<CreateMetodoPagoDto>
{
    public CreateMetodoPagoDtoValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre del método de pago es obligatorio")
            .MaximumLength(100)
            .WithMessage("El nombre no puede exceder 100 caracteres")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s\-]+$")
            .WithMessage("El nombre solo puede contener letras, espacios y guiones");
    }
}
