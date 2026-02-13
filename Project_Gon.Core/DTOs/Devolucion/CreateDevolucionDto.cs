namespace Project_Gon.Core.DTOs.Devolucion;

public class CreateDevolucionDto
{
    public int VentaId { get; set; }
    public int UsuarioId { get; set; }
    public string? Motivo { get; set; }
    public decimal MontoReembolso { get; set; }
    public List<CreateDetalleDevolucionDto> Detalles { get; set; } = new();
}