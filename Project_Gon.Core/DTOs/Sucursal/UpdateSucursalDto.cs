namespace Project_Gon.Core.DTOs.Sucursal;

/// <summary>
/// DTO para actualizar una sucursal existente.
/// Validaciones manejadas por FluentValidation (UpdateSucursalDtoValidator).
/// </summary>
public class UpdateSucursalDto
{
    public string Nombre { get; set; } = null!;
    public string? Region { get; set; }
    public string? Ciudad { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public bool Activo { get; set; } = true;
}