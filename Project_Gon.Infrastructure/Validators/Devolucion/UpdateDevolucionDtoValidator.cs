namespace Project_Gon.Infrastructure.Validators.Devolucion;

using FluentValidation;
using Project_Gon.Core.DTOs.Devolucion;

public class UpdateDevolucionDtoValidator : AbstractValidator<UpdateDevolucionDto>
{
    public UpdateDevolucionDtoValidator()
    {
        RuleFor(x => x.MontoReembolso)
            .GreaterThan(0)
            .When(x => x.MontoReembolso.HasValue)
            .WithMessage("El monto de reembolso debe ser mayor a 0.")
            .LessThanOrEqualTo(100000000)
            .When(x => x.MontoReembolso.HasValue)
            .WithMessage("El monto no puede exceder $100.000.000.");

        RuleFor(x => x.Motivo)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Motivo))
            .WithMessage("El motivo no puede exceder 500 caracteres.");

        RuleFor(x => x.Estado)
            .IsInEnum()
            .When(x => x.Estado.HasValue)
            .WithMessage("Estado de devolución inválido.");
    }
}