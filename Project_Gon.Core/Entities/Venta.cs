namespace Project_Gon.Core.Entities;

using Project_Gon.Core.Enums;

public class Venta
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public int SucursalId { get; set; }
    public int? ClienteId { get; set; }
    public int UsuarioId { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Iva { get; set; }
    public decimal Total { get; set; }
    public TipoVenta Tipo { get; set; }
    public EstadoVenta Estado { get; set; }
    public string? NumeroComprobante { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Relaciones
    public virtual Empresa Empresa { get; set; } = null!;
    public virtual Sucursal Sucursal { get; set; } = null!;
    public virtual Cliente? Cliente { get; set; }
    public virtual Usuario Usuario { get; set; } = null!;
}