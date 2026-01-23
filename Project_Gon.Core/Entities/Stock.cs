namespace Project_Gon.Core.Entities;

public class Stock
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public int SucursalId { get; set; }
    public int Cantidad { get; set; }
    public int StockMinimo { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Relaciones
    public virtual Producto Producto { get; set; } = null!;
    public virtual Sucursal Sucursal { get; set; } = null!;
    public virtual ICollection<MovimientoStock> Movimientos { get; set; } = new List<MovimientoStock>();
}