namespace Project_Gon.Core.DTOs.Terminal;

public class CreateTerminalDto
{
    public int SucursalId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? NumeroSerie { get; set; }
    public bool Activo { get; set; } = true;
}