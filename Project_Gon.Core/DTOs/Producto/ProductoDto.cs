namespace Project_Gon.Core.DTOs.Producto;

/// <summary>
/// DTO para lectura de Producto
/// </summary>
public class ProductoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public string? Sku { get; set; }
    public bool Activo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Relaciones
    public int EmpresaId { get; set; }
    public string? EmpresaNombre { get; set; }

    public int CategoriaId { get; set; }
    public string? CategoriaNombre { get; set; }
}