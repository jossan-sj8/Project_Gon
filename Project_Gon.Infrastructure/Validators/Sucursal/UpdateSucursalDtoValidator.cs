using FluentValidation;
using Project_Gon.Core.DTOs.Sucursal;

namespace Project_Gon.Infrastructure.Validators.Sucursal;

/// <summary>
/// Validador para la actualización de sucursales
/// </summary>
public class UpdateSucursalDtoValidator : AbstractValidator<UpdateSucursalDto>
{
    public UpdateSucursalDtoValidator()
    {
        // Nombre: obligatorio, máximo 255 caracteres
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre de la sucursal es obligatorio")
            .MaximumLength(255).WithMessage("El nombre no puede exceder 255 caracteres")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ0-9\s\.\-#]+$")
            .WithMessage("El nombre contiene caracteres inválidos");

        // Región: opcional, máximo 100 caracteres
        RuleFor(x => x.Region)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Region))
            .WithMessage("La región no puede exceder 100 caracteres");

        // Ciudad: opcional, máximo 100 caracteres
        RuleFor(x => x.Ciudad)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Ciudad))
            .WithMessage("La ciudad no puede exceder 100 caracteres");

        // Dirección: opcional, máximo 500 caracteres
        RuleFor(x => x.Direccion)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Direccion))
            .WithMessage("La dirección no puede exceder 500 caracteres");

        // Teléfono: formato válido si existe
        RuleFor(x => x.Telefono)
            .Matches(@"^\+?[0-9\s\-()]+$")
            .When(x => !string.IsNullOrWhiteSpace(x.Telefono))
            .WithMessage("Formato de teléfono inválido. Use: +56912345678 o 912345678")
            .MaximumLength(20)
            .When(x => !string.IsNullOrWhiteSpace(x.Telefono))
            .WithMessage("El teléfono no puede exceder 20 caracteres");

        // Activo: sin validaciones adicionales (es un bool)
    }
}