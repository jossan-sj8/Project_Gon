namespace Project_Gon.Core.DTOs.Stock;

/// <summary>
/// DTO para actualizar un registro de stock existente.
/// </summary>
public class UpdateStockDto
{
    /// <summary>
    /// Nueva cantidad. Opcional, pero si se proporciona debe ser >= 0.
    /// </summary>
    public int? Cantidad { get; set; }

    /// <summary>
    /// Nuevo stock mínimo. Opcional, pero si se proporciona debe ser > 0.
    /// </summary>
    public int? StockMinimo { get; set; }

    /// <summary>
    /// Nueva fecha de vencimiento. Opcional.
    /// </summary>
    public DateTime? FechaVencimiento { get; set; }
}