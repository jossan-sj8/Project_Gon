namespace Project_Gon.Core.Entities;

public class DetalleArqueoCaja
{
    public int Id { get; set; }
    public int ArqueoCajaId { get; set; }
    public int MetodoPagoId { get; set; }
    public decimal MontoTotal { get; set; }
    public int CantidadTransacciones { get; set; }

    // Relaciones
    public virtual ArqueoCaja ArqueoCaja { get; set; } = null!;
    public virtual MetodoPago MetodoPago { get; set; } = null!;
}