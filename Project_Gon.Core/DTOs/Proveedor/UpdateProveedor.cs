namespace Project_Gon.Core.DTOs.Proveedor;

/// <summary>
/// DTO para actualizar un proveedor existente
/// Todos los campos son opcionales
/// </summary>
public class UpdateProveedorDto
{
    public string? Nombre { get; set; }
    public string? Rut { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public string? Ciudad { get; set; }
    public bool? Activo { get; set; }
}