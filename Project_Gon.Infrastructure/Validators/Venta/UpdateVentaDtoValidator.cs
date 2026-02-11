using FluentValidation;
using Project_Gon.Core.DTOs.Venta;
using Project_Gon.Core.Enums;

namespace Project_Gon.Infrastructure.Validators.Venta;

public class UpdateVentaDtoValidator : AbstractValidator<UpdateVentaDto>
{
    public UpdateVentaDtoValidator()
    {
        RuleFor(x => x.Estado)
            .IsInEnum()
            .When(x => x.Estado.HasValue)
            .WithMessage("El estado de la venta debe ser válido (Completada, Pendiente, Cancelada)");

        RuleFor(x => x.NumeroComprobante)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.NumeroComprobante))
            .WithMessage("El número de comprobante no puede exceder 50 caracteres");
    }
}