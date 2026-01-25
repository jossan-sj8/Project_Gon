namespace Project_Gon.Core.Entities;

public class MetodoPago
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    // Relaciones
    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    public virtual ICollection<DetalleArqueoCaja> DetallesArqueo { get; set; } = new List<DetalleArqueoCaja>();
}