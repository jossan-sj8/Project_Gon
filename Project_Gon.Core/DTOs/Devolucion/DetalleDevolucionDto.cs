namespace Project_Gon.Core.DTOs.Devolucion;

public class DetalleDevolucionDto
{
    public int Id { get; set; }
    public int DevolucionId { get; set; }
    public int ProductoId { get; set; }
    public string? ProductoNombre { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public DateTime CreatedAt { get; set; }
}

