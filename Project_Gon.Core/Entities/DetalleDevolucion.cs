namespace Project_Gon.Core.Entities;

public class DetalleDevolucion
{
    public int Id { get; set; }
    public int DevolcionId { get; set; }
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public DateTime CreatedAt { get; set; }

    // Relaciones
    public virtual Devolucion Devolucion { get; set; } = null!;
    public virtual Producto Producto { get; set; } = null!;
}