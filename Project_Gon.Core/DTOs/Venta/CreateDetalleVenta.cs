namespace Project_Gon.Core.DTOs.Venta;

/// <summary>
/// DTO para agregar un producto a la venta
/// </summary>
public class CreateDetalleVentaDto
{
    /// <summary>
    /// ID del producto a vender
    /// </summary>
    public int ProductoId { get; set; }

    /// <summary>
    /// Cantidad a vender (debe haber stock disponible)
    /// </summary>
    public int Cantidad { get; set; }

    /// <summary>
    /// Precio unitario del producto (se obtiene automáticamente si no se proporciona)
    /// </summary>
    public decimal? PrecioUnitario { get; set; }
}