namespace Project_Gon.Core.DTOs.Venta;

using Project_Gon.Core.Enums;

/// <summary>
/// DTO para lectura de Venta con información completa
/// </summary>
public class VentaDto
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public string EmpresaNombre { get; set; } = string.Empty;
    public int SucursalId { get; set; }
    public string SucursalNombre { get; set; } = string.Empty;
    public int? ClienteId { get; set; }
    public string? ClienteNombre { get; set; }
    public string? ClienteRut { get; set; }
    public int UsuarioId { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal Iva { get; set; }
    public decimal Total { get; set; }
    public TipoVenta Tipo { get; set; }
    public string TipoDescripcion { get; set; } = string.Empty;
    public EstadoVenta Estado { get; set; }
    public string EstadoDescripcion { get; set; } = string.Empty;
    public string? NumeroComprobante { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Detalles de la venta (productos vendidos)
    /// </summary>
    public List<DetalleVentaDto> Detalles { get; set; } = new();
}