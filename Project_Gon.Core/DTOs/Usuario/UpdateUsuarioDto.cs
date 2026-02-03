using Project_Gon.Core.Enums;

namespace Project_Gon.Core.DTOs.Usuario;

/// <summary>
/// DTO para actualizar un usuario existente.
/// Validaciones manejadas por FluentValidation (UpdateUsuarioDtoValidator).
/// </summary>
public class UpdateUsuarioDto
{
    public string? Email { get; set; }
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public string? Password { get; set; }  // Solo si se quiere cambiar
    public RolUsuario? Rol { get; set; }
    public int? SucursalId { get; set; }
    public bool? Activo { get; set; }
}