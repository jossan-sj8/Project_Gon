namespace Project_Gon.Core.DTOs.Cliente;

/// <summary>
/// DTO para crear un nuevo Cliente
/// </summary>
public class CreateClienteDto
{
    public string Nombre { get; set; } = null!;
    public string? Rut { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public bool EsPublico { get; set; } = false;
    public int EmpresaId { get; set; }
}