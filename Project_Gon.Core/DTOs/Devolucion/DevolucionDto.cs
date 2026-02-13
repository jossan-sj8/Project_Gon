namespace Project_Gon.Core.DTOs.Devolucion;

using Project_Gon.Core.Enums;

public class DevolucionDto
{
    public int Id { get; set; }
    public int VentaId { get; set; }
    public int UsuarioId { get; set; }
    public string? UsuarioNombre { get; set; }
    public string? Motivo { get; set; }
    public decimal MontoReembolso { get; set; }
    public EstadoDevolucion Estado { get; set; }
    public List<DetalleDevolucionDto> Detalles { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}