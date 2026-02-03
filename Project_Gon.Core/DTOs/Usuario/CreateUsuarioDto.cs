using Project_Gon.Core.Enums;

namespace Project_Gon.Core.DTOs.Usuario;

/// <summary>
/// DTO para crear un nuevo usuario.
/// Validaciones manejadas por FluentValidation (CreateUsuarioDtoValidator).
/// </summary>
public class CreateUsuarioDto
{
    public string Rut { get; set; } = null!;
    public string? Email { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Password { get; set; } = null!;
    public RolUsuario Rol { get; set; }
    public int EmpresaId { get; set; }
    public int? SucursalId { get; set; }
}