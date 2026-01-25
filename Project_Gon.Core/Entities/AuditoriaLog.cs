namespace Project_Gon.Core.Entities;

public class AuditoriaLog
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string Entidad { get; set; } = null!;
    public int EntidadId { get; set; }
    public string Accion { get; set; } = null!;
    public string? ValoresAnterior { get; set; }
    public string? ValoresNuevo { get; set; }
    public string? Descripcion { get; set; }
    public DateTime CreatedAt { get; set; }

    // Relaciones
    public virtual Usuario Usuario { get; set; } = null!;
}