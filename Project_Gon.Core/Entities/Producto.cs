namespace Project_Gon.Core.Entities;

public class Producto
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public int CategoriaId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public string? Sku { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool Activo { get; set; } = true;

    // Relaciones
    public virtual Empresa Empresa { get; set; } = null!;
    public virtual Categoria Categoria { get; set; } = null!;
    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
    public virtual ICollection<DetalleVenta> DetalleVentas { get; set; } = new List<DetalleVenta>();
    public virtual ICollection<PrecioProveedor> PreciosProveedor { get; set; } = new List<PrecioProveedor>();
    public virtual ICollection<DetalleDevolucion> DetallesDevoluciones { get; set; } = new List<DetalleDevolucion>();
}