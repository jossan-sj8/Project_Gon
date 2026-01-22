namespace Project_Gon.Core.Entities;

public class Empresa
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Rut { get; set; }
    public string? Region { get; set; }
    public string? Ciudad { get; set; }
    public string? Direccion { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool Activo { get; set; } = true;
}