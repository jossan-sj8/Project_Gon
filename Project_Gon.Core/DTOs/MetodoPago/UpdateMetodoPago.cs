namespace Project_Gon.Core.DTOs.MetodoPago;

/// <summary>
/// DTO para actualizar un método de pago existente
/// </summary>
public class UpdateMetodoPagoDto
{
    public string? Nombre { get; set; }
    public bool? Activo { get; set; }
}