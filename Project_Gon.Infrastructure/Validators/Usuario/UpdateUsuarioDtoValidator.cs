using FluentValidation;
using Project_Gon.Core.DTOs.Usuario;
using Project_Gon.Core.Enums;

namespace Project_Gon.Infrastructure.Validators.Usuario;

/// <summary>
/// Validador para la actualización de usuarios.
/// Los campos son opcionales pero si se envían deben ser válidos.
/// </summary>
public class UpdateUsuarioDtoValidator : AbstractValidator<UpdateUsuarioDto>
{
    public UpdateUsuarioDtoValidator()
    {
        // Email: Opcional, pero si existe debe ser válido
        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("El formato del email es inválido")
            .MaximumLength(255)
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("El email no puede exceder 255 caracteres");

        // Nombre: Opcional, pero si existe debe ser válido
        RuleFor(x => x.Nombre)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Nombre))
            .WithMessage("El nombre no puede exceder 100 caracteres")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")
            .When(x => !string.IsNullOrWhiteSpace(x.Nombre))
            .WithMessage("El nombre solo puede contener letras y espacios");

        // Apellido: Opcional, pero si existe debe ser válido
        RuleFor(x => x.Apellido)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Apellido))
            .WithMessage("El apellido no puede exceder 100 caracteres")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")
            .When(x => !string.IsNullOrWhiteSpace(x.Apellido))
            .WithMessage("El apellido solo puede contener letras y espacios");

        // Password: Opcional, pero si se cambia debe cumplir requisitos de seguridad
        RuleFor(x => x.Password)
            .MinimumLength(8)
            .When(x => !string.IsNullOrWhiteSpace(x.Password))
            .WithMessage("La contraseña debe tener al menos 8 caracteres")
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Password))
            .WithMessage("La contraseña no puede exceder 100 caracteres")
            .Matches(@"[A-Z]")
            .When(x => !string.IsNullOrWhiteSpace(x.Password))
            .WithMessage("Debe contener al menos una letra mayúscula")
            .Matches(@"[a-z]")
            .When(x => !string.IsNullOrWhiteSpace(x.Password))
            .WithMessage("Debe contener al menos una letra minúscula")
            .Matches(@"[0-9]")
            .When(x => !string.IsNullOrWhiteSpace(x.Password))
            .WithMessage("Debe contener al menos un número")
            .Matches(@"[\W_]")
            .When(x => !string.IsNullOrWhiteSpace(x.Password))
            .WithMessage("Debe contener al menos un carácter especial (!@#$%^&* etc.)");

        // Rol: Opcional, pero si existe debe ser válido
        RuleFor(x => x.Rol)
            .IsInEnum()
            .When(x => x.Rol.HasValue)
            .WithMessage("El rol seleccionado no es válido");

        // SucursalId: Opcional, pero si existe debe ser mayor a 0
        RuleFor(x => x.SucursalId)
            .GreaterThan(0)
            .When(x => x.SucursalId.HasValue)
            .WithMessage("La sucursal seleccionada no es válida");
    }
}