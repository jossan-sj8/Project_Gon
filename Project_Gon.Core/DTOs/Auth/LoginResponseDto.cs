namespace Project_Gon.Core.DTOs.Auth;

/// <summary>
/// DTO de respuesta después de un login o registro exitoso
/// Contiene el token JWT y la información del usuario
/// </summary>
public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public UserInfoDto User { get; set; } = null!;
}