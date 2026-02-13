namespace Project_Gon.Infrastructure.Validators.Transaccion;

using FluentValidation;
using Project_Gon.Core.DTOs.Transaccion;

public class CreatePagoDtoValidator : AbstractValidator<CreatePagoDto>
{
    public CreatePagoDtoValidator()
    {
        RuleFor(x => x.VentaId)
            .GreaterThan(0)
            .WithMessage("El ID de la venta es requerido.");

        RuleFor(x => x.MetodoPagoId)
            .GreaterThan(0)
            .WithMessage("El método de pago es requerido.");

        RuleFor(x => x.Monto)
            .GreaterThan(0)
            .WithMessage("El monto debe ser mayor a 0.")
            .LessThanOrEqualTo(100000000)
            .WithMessage("El monto no puede exceder $100.000.000.");

        RuleFor(x => x.ReferenciaPago)
            .MaximumLength(100)
            .WithMessage("La referencia de pago no puede exceder 100 caracteres.");

        RuleFor(x => x.Estado)
            .IsInEnum()
            .WithMessage("Estado de pago inválido.");
    }
}