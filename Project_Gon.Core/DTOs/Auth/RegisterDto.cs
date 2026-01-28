using Project_Gon.Core.Enums;

namespace Project_Gon.Core.DTOs.Auth;

/// <summary>
/// DTO para registro de nuevos usuarios.
/// Validaciones manejadas por FluentValidation (RegisterDtoValidator).
/// </summary>
public class RegisterDto
{
    public string Rut { get; set; } = null!;
    public string? Email { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int EmpresaId { get; set; }
    public int? SucursalId { get; set; }
    public RolUsuario? Rol { get; set; }
}