namespace Project_Gon.Core.DTOs.MovimientoStock;

using Project_Gon.Core.Enums;

public class MovimientoStockDto
{
    public int Id { get; set; }
    public int StockId { get; set; }
    public int ProductoId { get; set; }
    public string? ProductoNombre { get; set; }
    public int SucursalId { get; set; }
    public string? SucursalNombre { get; set; }
    public int Cantidad { get; set; }
    public TipoMovimientoStock Tipo { get; set; }
    public string Motivo { get; set; } = null!;
    public int? UsuarioId { get; set; }
    public string? UsuarioNombre { get; set; }
    public int? VentaId { get; set; }
    public int? DevolucionId { get; set; }
    public DateTime CreatedAt { get; set; }
}