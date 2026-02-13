namespace Project_Gon.Core.DTOs.Transaccion;

using Project_Gon.Core.Enums;

public class PagoDto
{
    public int Id { get; set; }
    public int VentaId { get; set; }
    public int MetodoPagoId { get; set; }
    public string? MetodoPagoNombre { get; set; }
    public decimal Monto { get; set; }
    public string? ReferenciaPago { get; set; }
    public EstadoPago Estado { get; set; }
    public DateTime CreatedAt { get; set; }
}