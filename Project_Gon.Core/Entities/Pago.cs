namespace Project_Gon.Core.Entities;

using Project_Gon.Core.Enums;

public class Pago
{
    public int Id { get; set; }
    public int VentaId { get; set; }
    public int MetodoPagoId { get; set; }
    public decimal Monto { get; set; }
    public string? ReferenciaPago { get; set; }
    public EstadoPago Estado { get; set; }
    public DateTime CreatedAt { get; set; }

    // Relaciones
    public virtual Venta Venta { get; set; } = null!;
    public virtual MetodoPago MetodoPago { get; set; } = null!;
}