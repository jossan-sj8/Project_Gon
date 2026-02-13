namespace Project_Gon.Core.DTOs.Devolucion;

using Project_Gon.Core.Enums;

public class UpdateDevolucionDto
{
    public string? Motivo { get; set; }
    public decimal? MontoReembolso { get; set; }
    public EstadoDevolucion? Estado { get; set; }
}