namespace Project_Gon.Core.DTOs.Sucursal;

/// <summary>
/// DTO para retornar datos de una sucursal
/// </summary>
public class SucursalDto
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public string EmpresaNombre { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Region { get; set; }
    public string? Ciudad { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public bool Activo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}