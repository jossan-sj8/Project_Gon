using FluentValidation;
using Project_Gon.Core.DTOs.Auth;

namespace Project_Gon.Infrastructure.Validators.Auth;

/// <summary>
/// Validador para el login de usuarios
/// </summary>
public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        // EmailOrRut: obligatorio (puede ser RUT o Email)
        RuleFor(x => x.EmailOrRut)
            .NotEmpty().WithMessage("Debe ingresar un RUT o Email")
            .MaximumLength(100).WithMessage("El identificador no puede exceder 100 caracteres");

        // Password: obligatorio, mínimo 6 caracteres
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres");
    }
}
