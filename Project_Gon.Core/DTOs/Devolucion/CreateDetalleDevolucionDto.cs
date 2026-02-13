namespace Project_Gon.Core.DTOs.Devolucion;

public class CreateDetalleDevolucionDto
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}