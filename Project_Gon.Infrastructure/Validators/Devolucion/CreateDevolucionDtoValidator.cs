namespace Project_Gon.Infrastructure.Validators.Devolucion;

using FluentValidation;
using Project_Gon.Core.DTOs.Devolucion;

public class CreateDevolucionDtoValidator : AbstractValidator<CreateDevolucionDto>
{
    public CreateDevolucionDtoValidator()
    {
        RuleFor(x => x.VentaId)
            .GreaterThan(0)
            .WithMessage("El ID de la venta es requerido.");

        RuleFor(x => x.UsuarioId)
            .GreaterThan(0)
            .WithMessage("El ID del usuario es requerido.");

        RuleFor(x => x.MontoReembolso)
            .GreaterThan(0)
            .WithMessage("El monto de reembolso debe ser mayor a 0.")
            .LessThanOrEqualTo(100000000)
            .WithMessage("El monto no puede exceder $100.000.000.");

        RuleFor(x => x.Motivo)
            .MaximumLength(500)
            .WithMessage("El motivo no puede exceder 500 caracteres.");

        RuleFor(x => x.Detalles)
            .NotEmpty()
            .WithMessage("Debe incluir al menos un producto en la devolución.")
            .Must(d => d != null && d.Count > 0)
            .WithMessage("La devolución debe contener al menos un detalle.");

        RuleForEach(x => x.Detalles).SetValidator(new CreateDetalleDevolucionDtoValidator());
    }
}

public class CreateDetalleDevolucionDtoValidator : AbstractValidator<CreateDetalleDevolucionDto>
{
    public CreateDetalleDevolucionDtoValidator()
    {
        RuleFor(x => x.ProductoId)
            .GreaterThan(0)
            .WithMessage("El ID del producto es requerido.");

        RuleFor(x => x.Cantidad)
            .GreaterThan(0)
            .WithMessage("La cantidad debe ser mayor a 0.")
            .LessThanOrEqualTo(10000)
            .WithMessage("La cantidad no puede exceder 10.000 unidades.");

        RuleFor(x => x.PrecioUnitario)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El precio unitario no puede ser negativo.")
            .LessThanOrEqualTo(100000000)
            .WithMessage("El precio no puede exceder $100.000.000.");
    }
}