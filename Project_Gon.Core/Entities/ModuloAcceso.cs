namespace Project_Gon.Core.Entities;

public class ModuloAcceso
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string Modulo { get; set; } = null!;
    public bool CanCreate { get; set; }
    public bool CanRead { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
    public DateTime CreatedAt { get; set; }

    // Relaciones
    public virtual Usuario Usuario { get; set; } = null!;
}