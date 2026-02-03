namespace Project_Gon.Core.DTOs.Producto;

/// <summary>
/// DTO para crear un nuevo Producto
/// </summary>
public class CreateProductoDto
{
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public string? Sku { get; set; }
    public int EmpresaId { get; set; }
    public int CategoriaId { get; set; }
}