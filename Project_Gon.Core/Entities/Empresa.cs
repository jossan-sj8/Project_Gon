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

    public virtual ICollection<Sucursal> Sucursales { get; set; } = new List<Sucursal>();
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    public virtual ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();
    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
    public virtual ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    public virtual ICollection<Proveedor> Proveedores { get; set; } = new List<Proveedor>();
}