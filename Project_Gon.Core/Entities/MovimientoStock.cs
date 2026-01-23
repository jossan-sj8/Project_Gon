namespace Project_Gon.Core.Entities;

using Project_Gon.Core.Enums;

public class MovimientoStock
{
    public int Id { get; set; }
    public int StockId { get; set; }
    public int Cantidad { get; set; }
    public TipoMovimientoStock Tipo { get; set; }
    public string Motivo { get; set; } = null!;
    public int? UsuarioId { get; set; }
    public int? VentaId { get; set; }
    public int? DevolcionId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Relaciones
    public virtual Stock Stock { get; set; } = null!;
    public virtual Usuario? Usuario { get; set; }
    public virtual Venta? Venta { get; set; }
    public virtual Devolucion? Devolucion { get; set; }
}