namespace Project_Gon.Core.DTOs.Transaccion;

using Project_Gon.Core.Enums;

public class CreatePagoDto
{
    public int VentaId { get; set; }
    public int MetodoPagoId { get; set; }
    public decimal Monto { get; set; }
    public string? ReferenciaPago { get; set; }
    public EstadoPago Estado { get; set; } = EstadoPago.Completado;
}