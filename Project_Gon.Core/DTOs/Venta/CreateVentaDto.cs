namespace Project_Gon.Core.DTOs.Venta;

using Project_Gon.Core.Enums;

/// <summary>
/// DTO para crear una nueva venta
/// IMPORTANTE:
/// - Los detalles son OBLIGATORIOS (mínimo 1 producto)
/// - El stock se valida y descuenta automáticamente
/// - SubTotal, IVA y Total se calculan automáticamente
/// </summary>
public class CreateVentaDto
{
    /// <summary>
    /// ID de la empresa (se obtiene del usuario autenticado)
    /// </summary>
    public int EmpresaId { get; set; }

    /// <summary>
    /// ID de la sucursal donde se realiza la venta
    /// </summary>
    public int SucursalId { get; set; }

    /// <summary>
    /// ID del cliente (opcional para ventas anónimas)
    /// </summary>
    public int? ClienteId { get; set; }

    /// <summary>
    /// Tipo de venta: Presencial u Online
    /// </summary>
    public TipoVenta Tipo { get; set; } = TipoVenta.Presencial;

    /// <summary>
    /// Número de boleta/factura (opcional)
    /// </summary>
    public string? NumeroComprobante { get; set; }

    /// <summary>
    /// Detalles de productos vendidos (mínimo 1)
    /// </summary>
    public List<CreateDetalleVentaDto> Detalles { get; set; } = new();
}