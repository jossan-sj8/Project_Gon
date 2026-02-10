using FluentValidation;
using Project_Gon.Core.DTOs.Stock;

namespace Project_Gon.Infrastructure.Validators.Stock;

/// <summary>
/// Validador para CreateStockDto.
/// Valida que los datos de creación de stock sean correctos.
/// </summary>
public class CreateStockDtoValidator : AbstractValidator<CreateStockDto>
{
    public CreateStockDtoValidator()
    {
        // ProductoId
        RuleFor(x => x.ProductoId)
            .GreaterThan(0)
            .WithMessage("El ProductoId debe ser mayor a 0");

        // SucursalId
        RuleFor(x => x.SucursalId)
            .GreaterThan(0)
            .WithMessage("El SucursalId debe ser mayor a 0");

        // Cantidad
        RuleFor(x => x.Cantidad)
            .GreaterThanOrEqualTo(0)
            .WithMessage("La cantidad debe ser mayor o igual a 0")
            .LessThanOrEqualTo(999999)
            .WithMessage("La cantidad no puede exceder 999.999 unidades");

        // StockMinimo
        RuleFor(x => x.StockMinimo)
            .GreaterThan(0)
            .WithMessage("El stock mínimo debe ser mayor a 0")
            .LessThanOrEqualTo(999999)
            .WithMessage("El stock mínimo no puede exceder 999.999 unidades");

        // Cantidad debe ser >= StockMinimo (validación cruzada)
        RuleFor(x => x.Cantidad)
            .GreaterThanOrEqualTo(x => x.StockMinimo)
            .WithMessage("La cantidad inicial debe ser mayor o igual al stock mínimo")
            .When(x => x.StockMinimo > 0);

        // FechaVencimiento (opcional)
        RuleFor(x => x.FechaVencimiento)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("La fecha de vencimiento debe ser en el futuro")
            .When(x => x.FechaVencimiento.HasValue);
    }
}