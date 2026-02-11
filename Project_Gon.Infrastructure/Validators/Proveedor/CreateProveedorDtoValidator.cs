using FluentValidation;
using Project_Gon.Core.DTOs.Proveedor;

namespace Project_Gon.Infrastructure.Validators.Proveedor;

public class CreateProveedorDtoValidator : AbstractValidator<CreateProveedorDto>
{
    public CreateProveedorDtoValidator()
    {
        RuleFor(x => x.EmpresaId)
            .GreaterThan(0)
            .WithMessage("El ID de la empresa es obligatorio");

        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre del proveedor es obligatorio")
            .MaximumLength(255)
            .WithMessage("El nombre no puede exceder 255 caracteres")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ0-9\s\.\-&]+$")
            .WithMessage("El nombre solo puede contener letras, números, espacios y caracteres: . - &");

        RuleFor(x => x.Rut)
            .Matches(@"^\d{7,8}-[\dkK]$")
            .When(x => !string.IsNullOrEmpty(x.Rut))
            .WithMessage("El RUT debe tener formato válido: 12345678-9 o 12345678-K");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("El email debe tener un formato válido")
            .MaximumLength(255)
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("El email no puede exceder 255 caracteres");

        RuleFor(x => x.Telefono)
            .Matches(@"^\+?56\d{9}$")
            .When(x => !string.IsNullOrEmpty(x.Telefono))
            .WithMessage("El teléfono debe tener formato válido: +56912345678 o 56912345678");

        RuleFor(x => x.Direccion)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Direccion))
            .WithMessage("La dirección no puede exceder 500 caracteres");

        RuleFor(x => x.Ciudad)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Ciudad))
            .WithMessage("La ciudad no puede exceder 100 caracteres");
    }
}