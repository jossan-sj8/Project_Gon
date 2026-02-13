namespace Project_Gon.Infrastructure.Validators.MovimientoStock;

using FluentValidation;
using Project_Gon.Core.DTOs.MovimientoStock;

public class CreateMovimientoStockDtoValidator : AbstractValidator<CreateMovimientoStockDto>
{
    public CreateMovimientoStockDtoValidator()
    {
        RuleFor(x => x.StockId)
            .GreaterThan(0)
            .WithMessage("El ID del stock es requerido.");

        RuleFor(x => x.Cantidad)
            .GreaterThan(0)
            .WithMessage("La cantidad debe ser mayor a 0.")
            .LessThanOrEqualTo(100000)
            .WithMessage("La cantidad no puede exceder 100.000 unidades.");

        RuleFor(x => x.Tipo)
            .IsInEnum()
            .WithMessage("Tipo de movimiento inválido.");

        RuleFor(x => x.Motivo)
            .NotEmpty()
            .WithMessage("El motivo es requerido.")
            .MaximumLength(500)
            .WithMessage("El motivo no puede exceder 500 caracteres.");
    }
}