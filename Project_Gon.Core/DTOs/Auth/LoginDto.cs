namespace Project_Gon.Core.DTOs.Auth;

/// <summary>
/// DTO para login de usuarios.
/// Validaciones manejadas por FluentValidation (LoginDtoValidator).
/// </summary>
public class LoginDto
{
    /// <summary>
    /// Email o RUT del usuario (auto-detectado por el backend)
    /// </summary>
    public string EmailOrRut { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña del usuario
    /// </summary>
    public string Password { get; set; } = string.Empty;
}