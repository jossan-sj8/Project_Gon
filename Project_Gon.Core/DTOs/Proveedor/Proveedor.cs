namespace Project_Gon.Core.DTOs.Proveedor;

/// <summary>
/// DTO para lectura de Proveedor con información completa
/// </summary>
public class ProveedorDto
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public string EmpresaNombre { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Rut { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public string? Ciudad { get; set; }
    public bool Activo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}