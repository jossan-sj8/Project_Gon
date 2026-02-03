namespace Project_Gon.Core.DTOs.Cliente;

/// <summary>
/// DTO para actualizar un Cliente existente
/// </summary>
public class UpdateClienteDto
{
    public string? Nombre { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public bool? EsPublico { get; set; }
}