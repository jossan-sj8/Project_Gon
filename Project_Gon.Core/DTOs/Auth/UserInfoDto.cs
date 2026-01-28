namespace Project_Gon.Core.DTOs.Auth;

/// <summary>
/// DTO con información del usuario autenticado
/// Se devuelve en el LoginResponse después del login/register exitoso
/// </summary>
public class UserInfoDto
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public string? Rut { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Rol { get; set; } = null!;
    public int EmpresaId { get; set; }
    public string EmpresaNombre { get; set; } = null!;
    public int? SucursalId { get; set; }
    public string? SucursalNombre { get; set; }
}