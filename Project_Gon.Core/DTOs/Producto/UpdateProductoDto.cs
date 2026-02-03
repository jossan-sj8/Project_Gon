namespace Project_Gon.Core.DTOs.Producto;

/// <summary>
/// DTO para actualizar un Producto existente
/// </summary>
public class UpdateProductoDto
{
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public decimal? Precio { get; set; }
    public string? Sku { get; set; }
    public int? CategoriaId { get; set; }
}