namespace Project_Gon.Core.Entities;

public class Proveedor
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Rut { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public string? Ciudad { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Relaciones
    public virtual Empresa Empresa { get; set; } = null!;
    public virtual ICollection<PrecioProveedor> PreciosProveedor { get; set; } = new List<PrecioProveedor>();
}