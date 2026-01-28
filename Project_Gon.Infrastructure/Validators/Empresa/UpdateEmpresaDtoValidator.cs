using FluentValidation;
using Project_Gon.Core.DTOs.Empresa;

namespace Project_Gon.Infrastructure.Validators.Empresa;

/// <summary>
/// Validador para la actualización de empresas
/// </summary>
public class UpdateEmpresaDtoValidator : AbstractValidator<UpdateEmpresaDto>
{
    public UpdateEmpresaDtoValidator()
    {
        // Nombre: obligatorio, máximo 255 caracteres
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre de la empresa es obligatorio")
            .MaximumLength(255).WithMessage("El nombre no puede exceder 255 caracteres")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ0-9\s\.\-&]+$")
            .WithMessage("El nombre contiene caracteres inválidos");

        // RUT: opcional, pero si existe debe tener formato chileno
        RuleFor(x => x.Rut)
            .Matches(@"^\d{1,8}-[\dkK]$")
            .When(x => !string.IsNullOrWhiteSpace(x.Rut))
            .WithMessage("Formato de RUT inválido. Use formato: 12345678-9");

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

        // Activa: sin validaciones adicionales (es un bool)
    }
}