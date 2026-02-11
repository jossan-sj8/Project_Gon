namespace Project_Gon.Core.DTOs.MetodoPago;

/// <summary>
/// DTO para lectura de Método de Pago
/// </summary>
public class MetodoPagoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public DateTime CreatedAt { get; set; }
}