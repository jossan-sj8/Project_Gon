using FluentValidation;
using Project_Gon.Core.DTOs.MetodoPago;

namespace Project_Gon.Infrastructure.Validators.MetodoPago;

public class UpdateMetodoPagoDtoValidator : AbstractValidator<UpdateMetodoPagoDto>
{
    public UpdateMetodoPagoDtoValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty()
            .When(x => x.Nombre != null)
            .WithMessage("El nombre no puede estar vacío si se proporciona")
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Nombre))
            .WithMessage("El nombre no puede exceder 100 caracteres")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s\-]+$")
            .When(x => !string.IsNullOrEmpty(x.Nombre))
            .WithMessage("El nombre solo puede contener letras, espacios y guiones");
    }
}