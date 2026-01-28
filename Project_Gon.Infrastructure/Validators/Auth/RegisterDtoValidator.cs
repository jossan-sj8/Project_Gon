using FluentValidation;
using Project_Gon.Core.DTOs.Auth;
using Project_Gon.Core.Enums;

namespace Project_Gon.Infrastructure.Validators.Auth;

/// <summary>
/// Validador para el registro de nuevos usuarios
/// Incluye validaciones condicionales según el rol
/// </summary>
public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        // ====================================
        // RUT: Obligatorio con formato chileno
        // ====================================
        RuleFor(x => x.Rut)
            .NotEmpty().WithMessage("El RUT es obligatorio")
            .Matches(@"^\d{1,8}-[\dkK]$")
            .WithMessage("Formato de RUT inválido. Use formato sin puntos: 12345678-9 o 12345678-K");

        // ====================================
        // Email: Obligatorio SOLO si NO es Vendedor
        // ====================================
        RuleFor(x => x.Email)
            .NotEmpty()
            .When(x => x.Rol != RolUsuario.Vendedor)
            .WithMessage("El email es obligatorio para este rol")
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("El formato del email es inválido")
            .MaximumLength(255)
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("El email no puede exceder 255 caracteres");

        // ====================================
        // Nombre: Obligatorio, máximo 100 caracteres
        // ====================================
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")
            .WithMessage("El nombre solo puede contener letras y espacios");

        // ====================================
        // Apellido: Obligatorio, máximo 100 caracteres
        // ====================================
        RuleFor(x => x.Apellido)
            .NotEmpty().WithMessage("El apellido es obligatorio")
            .MaximumLength(100).WithMessage("El apellido no puede exceder 100 caracteres")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")
            .WithMessage("El apellido solo puede contener letras y espacios");

        // ====================================
        // Contraseña: Segura con múltiples validaciones
        // ====================================
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres")
            .MaximumLength(100).WithMessage("La contraseña no puede exceder 100 caracteres")
            .Matches(@"[A-Z]").WithMessage("Debe contener al menos una letra mayúscula")
            .Matches(@"[a-z]").WithMessage("Debe contener al menos una letra minúscula")
            .Matches(@"[0-9]").WithMessage("Debe contener al menos un número")
            .Matches(@"[\W_]").WithMessage("Debe contener al menos un carácter especial (!@#$%^&* etc.)");

        // ====================================
        // EmpresaId: Obligatorio y mayor a 0
        // ====================================
        RuleFor(x => x.EmpresaId)
            .GreaterThan(0).WithMessage("Debe seleccionar una empresa válida");

        // ====================================
        // SucursalId: Obligatorio SOLO si es Vendedor
        // ====================================
        RuleFor(x => x.SucursalId)
            .NotNull()
            .When(x => x.Rol == RolUsuario.Vendedor)
            .WithMessage("Los vendedores deben tener una sucursal asignada")
            .GreaterThan(0)
            .When(x => x.SucursalId.HasValue)
            .WithMessage("La sucursal seleccionada no es válida");

        // ====================================
        // Rol: Validar que sea un valor válido del enum
        // ====================================
        RuleFor(x => x.Rol)
            .IsInEnum()
            .When(x => x.Rol.HasValue)
            .WithMessage("El rol seleccionado no es válido");
    }
}