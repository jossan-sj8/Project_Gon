namespace Project_Gon.Core.Entities;

public class PrecioProveedor
{
    public int Id { get; set; }
    public int ProveedorId { get; set; }
    public int ProductoId { get; set; }
    public decimal Precio { get; set; }
    public DateTime FechaVigencia { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    // Relaciones
    public virtual Proveedor Proveedor { get; set; } = null!;
    public virtual Producto Producto { get; set; } = null!;
}