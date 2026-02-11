namespace Project_Gon.Core.DTOs.Venta;

/// <summary>
/// DTO para lectura de detalle de venta
/// </summary>
public class DetalleVentaDto
{
    public int Id { get; set; }
    public int VentaId { get; set; }
    public int ProductoId { get; set; }
    public string ProductoNombre { get; set; } = string.Empty;
    public string? ProductoCodigoBarra { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public DateTime CreatedAt { get; set; }
}