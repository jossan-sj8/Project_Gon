using Project_Gon.Core.Enums;

namespace Project_Gon.Core.DTOs.Usuario;

/// <summary>
/// DTO para lectura de información de usuarios.
/// Incluye información de empresa y sucursal.
/// </summary>
public class UsuarioDto
{
    public int Id { get; set; }
    public string Rut { get; set; } = null!;
    public string? Email { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string NombreCompleto => $"{Nombre} {Apellido}";
    public RolUsuario Rol { get; set; }
    public string RolNombre => Rol.ToString();

    // Relación con Empresa
    public int EmpresaId { get; set; }
    public string EmpresaNombre { get; set; } = null!;

    // Relación con Sucursal (opcional según rol)
    public int? SucursalId { get; set; }
    public string? SucursalNombre { get; set; }

    // Estado
    public bool Activo { get; set; }

    // Auditoría
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}