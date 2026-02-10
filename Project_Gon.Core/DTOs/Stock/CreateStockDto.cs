namespace Project_Gon.Core.DTOs.Stock;

/// <summary>
/// DTO para crear un nuevo registro de stock.
/// </summary>
public class CreateStockDto
{
    /// <summary>
    /// Identificador del producto. Requerido.
    /// </summary>
    public int ProductoId { get; set; }

    /// <summary>
    /// Identificador de la sucursal. Requerido.
    /// </summary>
    public int SucursalId { get; set; }

    /// <summary>
    /// Cantidad inicial del stock. Requerido, mínimo 0.
    /// </summary>
    public int Cantidad { get; set; }

    /// <summary>
    /// Stock mínimo permitido. Requerido, mayor a 0.
    /// </summary>
    public int StockMinimo { get; set; }

    /// <summary>
    /// Fecha de vencimiento del lote. Opcional.
    /// </summary>
    public DateTime? FechaVencimiento { get; set; }
}