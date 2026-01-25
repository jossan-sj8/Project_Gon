namespace Project_Gon.Core.Entities;

using Project_Gon.Core.Enums;

public class ArqueoCaja
{
    public int Id { get; set; }
    public int CajaRegistradoraId { get; set; }
    public int UsuarioId { get; set; }
    public DateTime FechaApertura { get; set; }
    public DateTime? FechaCierre { get; set; }
    public decimal SaldoInicial { get; set; }
    public decimal SaldoEsperado { get; set; }
    public decimal SaldoReal { get; set; }
    public decimal Diferencia { get; set; }
    public string? Observaciones { get; set; }
    public EstadoArqueoCaja Estado { get; set; }

    // Relaciones
    public virtual CajaRegistradora CajaRegistradora { get; set; } = null!;
    public virtual Usuario Usuario { get; set; } = null!;
    public virtual ICollection<DetalleArqueoCaja> Detalles { get; set; } = new List<DetalleArqueoCaja>();
}