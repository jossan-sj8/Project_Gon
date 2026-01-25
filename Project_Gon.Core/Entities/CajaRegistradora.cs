namespace Project_Gon.Core.Entities;

public class CajaRegistradora
{
    public int Id { get; set; }
    public int SucursalId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? NumeroSerie { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Relaciones
    public virtual Sucursal Sucursal { get; set; } = null!;
    public virtual ICollection<ArqueoCaja> Arqueos { get; set; } = new List<ArqueoCaja>();
}