namespace Project_Gon.Core.DTOs.MetodoPago;

/// <summary>
/// DTO para crear un nuevo método de pago
/// Ejemplos: Efectivo, Tarjeta Débito, Tarjeta Crédito, Transferencia, etc.
/// </summary>
public class CreateMetodoPagoDto
{
    /// <summary>
    /// Nombre del método de pago (obligatorio)
    /// </summary>
    public string Nombre { get; set; } = string.Empty;
}