namespace Project_Gon.Core.DTOs.Venta;

using Project_Gon.Core.Enums;

/// <summary>
/// DTO para actualizar una venta existente
/// IMPORTANTE:
/// - Solo se puede cambiar el estado y número de comprobante
/// - NO se pueden modificar detalles (usar devoluciones)
/// - NO se puede cambiar cliente, sucursal o empresa
/// </summary>
public class UpdateVentaDto
{
    /// <summary>
    /// Estado de la venta: Completada, Pendiente, Cancelada
    /// </summary>
    public EstadoVenta? Estado { get; set; }

    /// <summary>
    /// Número de boleta/factura
    /// </summary>
    public string? NumeroComprobante { get; set; }
}