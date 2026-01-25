namespace Project_Gon.Core.Entities;

using Project_Gon.Core.Enums;

public class Devolucion
{
    public int Id { get; set; }
    public int VentaId { get; set; }
    public int UsuarioId { get; set; }
    public string? Motivo { get; set; }
    public decimal MontoReembolso { get; set; }
    public EstadoDevolucion Estado { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Relaciones
    public virtual Venta Venta { get; set; } = null!;
    public virtual Usuario Usuario { get; set; } = null!;
    public virtual ICollection<DetalleDevolucion> Detalles { get; set; } = new List<DetalleDevolucion>();
}