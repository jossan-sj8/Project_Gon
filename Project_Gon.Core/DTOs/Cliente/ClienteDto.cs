namespace Project_Gon.Core.DTOs.Cliente;

/// <summary>
/// DTO para lectura de Cliente
/// </summary>
public class ClienteDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Rut { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public bool EsPublico { get; set; }
    public bool Activo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Relaciones
    public int EmpresaId { get; set; }
    public string? EmpresaNombre { get; set; }
}