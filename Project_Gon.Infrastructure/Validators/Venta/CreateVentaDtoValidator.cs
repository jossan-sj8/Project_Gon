using FluentValidation;
using Project_Gon.Core.DTOs.Venta;
using Project_Gon.Core.Enums;

namespace Project_Gon.Infrastructure.Validators.Venta;

public class CreateVentaDtoValidator : AbstractValidator<CreateVentaDto>
{
    public CreateVentaDtoValidator()
    {
        RuleFor(x => x.EmpresaId)
            .GreaterThan(0)
            .WithMessage("El ID de la empresa es obligatorio");

        RuleFor(x => x.SucursalId)
            .GreaterThan(0)
            .WithMessage("El ID de la sucursal es obligatorio");

        RuleFor(x => x.ClienteId)
            .GreaterThan(0)
            .When(x => x.ClienteId.HasValue)
            .WithMessage("El ID del cliente debe ser mayor a 0 si se proporciona");

        RuleFor(x => x.Tipo)
            .IsInEnum()
            .WithMessage("El tipo de venta debe ser válido (Presencial u Online)");

        RuleFor(x => x.NumeroComprobante)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.NumeroComprobante))
            .WithMessage("El número de comprobante no puede exceder 50 caracteres");

        RuleFor(x => x.Detalles)
            .NotEmpty()
            .WithMessage("La venta debe tener al menos un producto");

        RuleForEach(x => x.Detalles)
            .SetValidator(new CreateDetalleVentaDtoValidator());
    }
}

public class CreateDetalleVentaDtoValidator : AbstractValidator<CreateDetalleVentaDto>
{
    public CreateDetalleVentaDtoValidator()
    {
        RuleFor(x => x.ProductoId)
            .GreaterThan(0)
            .WithMessage("El ID del producto es obligatorio");

        RuleFor(x => x.Cantidad)
            .GreaterThan(0)
            .WithMessage("La cantidad debe ser mayor a 0")
            .LessThanOrEqualTo(10000)
            .WithMessage("La cantidad no puede exceder 10,000 unidades");

        RuleFor(x => x.PrecioUnitario)
            .GreaterThan(0)
            .When(x => x.PrecioUnitario.HasValue)
            .WithMessage("El precio unitario debe ser mayor a 0 si se proporciona");
    }
}