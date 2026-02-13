namespace Project_Gon.Core.DTOs.MovimientoStock;

using Project_Gon.Core.Enums;

public class CreateMovimientoStockDto
{
    public int StockId { get; set; }
    public int Cantidad { get; set; }
    public TipoMovimientoStock Tipo { get; set; }
    public string Motivo { get; set; } = null!;
    public int? UsuarioId { get; set; }
    public int? VentaId { get; set; }
    public int? DevolucionId { get; set; }
}