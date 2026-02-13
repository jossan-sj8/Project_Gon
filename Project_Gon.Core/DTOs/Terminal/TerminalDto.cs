namespace Project_Gon.Core.DTOs.Terminal;

public class TerminalDto
{
    public int Id { get; set; }
    public int SucursalId { get; set; }
    public string? SucursalNombre { get; set; }
    public string Nombre { get; set; } = null!;
    public string? NumeroSerie { get; set; }
    public bool Activo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}