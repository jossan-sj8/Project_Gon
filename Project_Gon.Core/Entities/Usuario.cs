namespace Project_Gon.Core.Entities;

using Project_Gon.Core.Enums;

public class Usuario
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public string? Email { get; set; }                    
    public string? Rut { get; set; }                     
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public RolUsuario Rol { get; set; }
    public int? SucursalId { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? UltimoAcceso { get; set; }

    // Relaciones
    public virtual Empresa Empresa { get; set; } = null!;
    public virtual Sucursal? Sucursal { get; set; }
    public virtual ICollection<MovimientoStock> MovimientosStock { get; set; } = new List<MovimientoStock>();
    public virtual ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    public virtual ICollection<ArqueoCaja> ArqueosCaja { get; set; } = new List<ArqueoCaja>();
    public virtual ICollection<Devolucion> Devoluciones { get; set; } = new List<Devolucion>();
    public virtual ICollection<AuditoriaLog> AuditLogs { get; set; } = new List<AuditoriaLog>();
    public virtual ICollection<ModuloAcceso> ModuloAccesos { get; set; } = new List<ModuloAcceso>();
}