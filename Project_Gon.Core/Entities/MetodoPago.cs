namespace Project_Gon.Core.Entities;

public class MetodoPago
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}