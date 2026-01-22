namespace Project_Gon.Core.Entities;

public class Sucursal
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Region { get; set; }
    public string? Ciudad { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool Activo { get; set; } = true;

    // Relación con Empresa
    public virtual Empresa Empresa { get; set; } = null!;
}
