namespace Project_Gon.Core.DTOs.Sucursal;

/// <summary>
/// DTO para crear una nueva sucursal.
/// Validaciones manejadas por FluentValidation (CreateSucursalDtoValidator).
/// </summary>
public class CreateSucursalDto
{
    public int EmpresaId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Region { get; set; }
    public string? Ciudad { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
}