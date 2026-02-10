namespace Project_Gon.Core.DTOs.Stock;

/// <summary>
/// DTO de lectura para Stock.
/// Representa el stock de un producto en una sucursal específica.
/// </summary>
public class StockDto
{
    /// <summary>
    /// Identificador único del stock.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del producto.
    /// </summary>
    public int ProductoId { get; set; }

    /// <summary>
    /// Nombre del producto (incluido para referencia en GET).
    /// </summary>
    public string ProductoNombre { get; set; } = null!;

    /// <summary>
    /// Identificador de la sucursal donde se encuentra el stock.
    /// </summary>
    public int SucursalId { get; set; }

    /// <summary>
    /// Nombre de la sucursal (incluido para referencia en GET).
    /// </summary>
    public string SucursalNombre { get; set; } = null!;

    /// <summary>
    /// Cantidad actual disponible.
    /// </summary>
    public int Cantidad { get; set; }

    /// <summary>
    /// Stock mínimo antes de generar alerta.
    /// </summary>
    public int StockMinimo { get; set; }

    /// <summary>
    /// Fecha de vencimiento del lote (opcional).
    /// </summary>
    public DateTime? FechaVencimiento { get; set; }

    /// <summary>
    /// Indica si el stock está bajo el mínimo.
    /// </summary>
    public bool EstaAgotado => Cantidad <= StockMinimo;

    /// <summary>
    /// Fecha de creación del registro.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Fecha de última actualización.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}