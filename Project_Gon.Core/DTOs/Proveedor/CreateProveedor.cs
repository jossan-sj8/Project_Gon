namespace Project_Gon.Core.DTOs.Proveedor;

/// <summary>
/// DTO para crear un nuevo proveedor
/// </summary>
public class CreateProveedorDto
{
    /// <summary>
    /// ID de la empresa (se obtiene del usuario autenticado)
    /// </summary>
    public int EmpresaId { get; set; }

    /// <summary>
    /// Nombre del proveedor (obligatorio)
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// RUT del proveedor (formato: 12345678-9)
    /// </summary>
    public string? Rut { get; set; }

    /// <summary>
    /// Email del proveedor
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Teléfono del proveedor (formato: +56912345678)
    /// </summary>
    public string? Telefono { get; set; }

    /// <summary>
    /// Dirección física del proveedor
    /// </summary>
    public string? Direccion { get; set; }

    /// <summary>
    /// Ciudad del proveedor
    /// </summary>
    public string? Ciudad { get; set; }
}