using FluentValidation;
using Project_Gon.Core.DTOs.Stock;

namespace Project_Gon.Infrastructure.Validators.Stock;

/// <summary>
/// Validador para UpdateStockDto.
/// Valida que los datos de actualización de stock sean correctos.
/// Todos los campos son opcionales.
/// </summary>
public class UpdateStockDtoValidator : AbstractValidator<UpdateStockDto>
{
    public UpdateStockDtoValidator()
    {
        // Cantidad (opcional)
        RuleFor(x => x.Cantidad)
            .GreaterThanOrEqualTo(0)
            .WithMessage("La cantidad debe ser mayor o igual a 0")
            .LessThanOrEqualTo(999999)
            .WithMessage("La cantidad no puede exceder 999.999 unidades")
            .When(x => x.Cantidad.HasValue);

        // StockMinimo (opcional)
        RuleFor(x => x.StockMinimo)
            .GreaterThan(0)
            .WithMessage("El stock mínimo debe ser mayor a 0")
            .LessThanOrEqualTo(999999)
            .WithMessage("El stock mínimo no puede exceder 999.999 unidades")
            .When(x => x.StockMinimo.HasValue);

        // FechaVencimiento (opcional)
        RuleFor(x => x.FechaVencimiento)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("La fecha de vencimiento debe ser en el futuro")
            .When(x => x.FechaVencimiento.HasValue);
    }
}