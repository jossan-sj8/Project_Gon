using Microsoft.EntityFrameworkCore;
using Project_Gon.Core.Entities;

namespace Project_Gon.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets - Entidades
    public DbSet<Empresa> Empresas { get; set; } = null!;
    public DbSet<Sucursal> Sucursales { get; set; } = null!;
    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<Categoria> Categorias { get; set; } = null!;
    public DbSet<Producto> Productos { get; set; } = null!;
    public DbSet<Stock> Stocks { get; set; } = null!;
    public DbSet<MovimientoStock> MovimientosStock { get; set; } = null!;
    public DbSet<Cliente> Clientes { get; set; } = null!;
    public DbSet<MetodoPago> MetodosPago { get; set; } = null!;
    public DbSet<Venta> Ventas { get; set; } = null!;
    public DbSet<DetalleVenta> DetallesVenta { get; set; } = null!;
    public DbSet<Pago> Pagos { get; set; } = null!;
    public DbSet<Proveedor> Proveedores { get; set; } = null!;
    public DbSet<PrecioProveedor> PreciosProveedor { get; set; } = null!;
    public DbSet<CajaRegistradora> CajasRegistradoras { get; set; } = null!;
    public DbSet<ArqueoCaja> ArqueosCaja { get; set; } = null!;
    public DbSet<DetalleArqueoCaja> DetallesArqueoCaja { get; set; } = null!;
    public DbSet<Devolucion> Devoluciones { get; set; } = null!;
    public DbSet<DetalleDevolucion> DetallesDevoluciones { get; set; } = null!;
    public DbSet<AuditoriaLog> AuditoriaLogs { get; set; } = null!;
    public DbSet<ModuloAcceso> ModulosAcceso { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de índices y constraints

        // Empresa
        modelBuilder.Entity<Empresa>()
            .HasKey(e => e.Id);
        modelBuilder.Entity<Empresa>()
            .Property(e => e.Nombre)
            .IsRequired()
            .HasMaxLength(255);

        // Sucursal
        modelBuilder.Entity<Sucursal>()
            .HasKey(s => s.Id);
        modelBuilder.Entity<Sucursal>()
            .HasOne(s => s.Empresa)
            .WithMany(e => e.Sucursales)
            .HasForeignKey(s => s.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Sucursal>()
            .Property(s => s.Nombre)
            .IsRequired()
            .HasMaxLength(255);

        // Usuario
        modelBuilder.Entity<Usuario>()
            .HasKey(u => u.Id);
        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Empresa)
            .WithMany(e => e.Usuarios)
            .HasForeignKey(u => u.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Sucursal)
            .WithMany(s => s.Usuarios)
            .HasForeignKey(u => u.SucursalId)
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Usuario>()
            .Property(u => u.Email)
            .HasMaxLength(255);
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => new { u.EmpresaId, u.Email })
            .IsUnique();

        // Categoria
        modelBuilder.Entity<Categoria>()
            .HasKey(c => c.Id);
        modelBuilder.Entity<Categoria>()
            .HasOne(c => c.Empresa)
            .WithMany(e => e.Categorias)
            .HasForeignKey(c => c.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Producto
        modelBuilder.Entity<Producto>()
            .HasKey(p => p.Id);
        modelBuilder.Entity<Producto>()
            .HasOne(p => p.Empresa)
            .WithMany(e => e.Productos)
            .HasForeignKey(p => p.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Producto>()
            .HasOne(p => p.Categoria)
            .WithMany(c => c.Productos)
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Stock
        modelBuilder.Entity<Stock>()
            .HasKey(s => s.Id);
        modelBuilder.Entity<Stock>()
            .HasOne(s => s.Producto)
            .WithMany(p => p.Stocks)
            .HasForeignKey(s => s.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Stock>()
            .HasOne(s => s.Sucursal)
            .WithMany(s => s.Stocks)
            .HasForeignKey(s => s.SucursalId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Stock>()
            .HasIndex(s => new { s.ProductoId, s.SucursalId })
            .IsUnique();

        // MovimientoStock
        modelBuilder.Entity<MovimientoStock>()
            .HasKey(m => m.Id);
        modelBuilder.Entity<MovimientoStock>()
            .HasOne(m => m.Stock)
            .WithMany(s => s.Movimientos)
            .HasForeignKey(m => m.StockId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<MovimientoStock>()
            .HasOne(m => m.Usuario)
            .WithMany(u => u.MovimientosStock)
            .HasForeignKey(m => m.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        // Cliente
        modelBuilder.Entity<Cliente>()
            .HasKey(c => c.Id);
        modelBuilder.Entity<Cliente>()
            .HasOne(c => c.Empresa)
            .WithMany(e => e.Clientes)
            .HasForeignKey(c => c.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);

        // MetodoPago
        modelBuilder.Entity<MetodoPago>()
            .HasKey(m => m.Id);

        // Venta
        modelBuilder.Entity<Venta>()
            .HasKey(v => v.Id);
        modelBuilder.Entity<Venta>()
            .HasOne(v => v.Empresa)
            .WithMany(e => e.Ventas)
            .HasForeignKey(v => v.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Venta>()
            .HasOne(v => v.Sucursal)
            .WithMany(s => s.Ventas)
            .HasForeignKey(v => v.SucursalId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Venta>()
            .HasOne(v => v.Cliente)
            .WithMany(c => c.Ventas)
            .HasForeignKey(v => v.ClienteId)
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Venta>()
            .HasOne(v => v.Usuario)
            .WithMany(u => u.Ventas)
            .HasForeignKey(v => v.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // DetalleVenta
        modelBuilder.Entity<DetalleVenta>()
            .HasKey(d => d.Id);
        modelBuilder.Entity<DetalleVenta>()
            .HasOne(d => d.Venta)
            .WithMany(v => v.Detalles)
            .HasForeignKey(d => d.VentaId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<DetalleVenta>()
            .HasOne(d => d.Producto)
            .WithMany(p => p.DetalleVentas)
            .HasForeignKey(d => d.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Pago
        modelBuilder.Entity<Pago>()
            .HasKey(p => p.Id);
        modelBuilder.Entity<Pago>()
            .HasOne(p => p.Venta)
            .WithMany(v => v.Pagos)
            .HasForeignKey(p => p.VentaId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Pago>()
            .HasOne(p => p.MetodoPago)
            .WithMany(m => m.Pagos)
            .HasForeignKey(p => p.MetodoPagoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Proveedor
        modelBuilder.Entity<Proveedor>()
            .HasKey(p => p.Id);
        modelBuilder.Entity<Proveedor>()
            .HasOne(p => p.Empresa)
            .WithMany(e => e.Proveedores)
            .HasForeignKey(p => p.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);

        // PrecioProveedor
        modelBuilder.Entity<PrecioProveedor>()
            .HasKey(p => p.Id);
        modelBuilder.Entity<PrecioProveedor>()
            .HasOne(p => p.Proveedor)
            .WithMany(p => p.PreciosProveedor)
            .HasForeignKey(p => p.ProveedorId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PrecioProveedor>()
            .HasOne(p => p.Producto)
            .WithMany(p => p.PreciosProveedor)
            .HasForeignKey(p => p.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        // CajaRegistradora
        modelBuilder.Entity<CajaRegistradora>()
            .HasKey(c => c.Id);
        modelBuilder.Entity<CajaRegistradora>()
            .HasOne(c => c.Sucursal)
            .WithMany(s => s.CajasRegistradoras)
            .HasForeignKey(c => c.SucursalId)
            .OnDelete(DeleteBehavior.Restrict);

        // ArqueoCaja
        modelBuilder.Entity<ArqueoCaja>()
            .HasKey(a => a.Id);
        modelBuilder.Entity<ArqueoCaja>()
            .HasOne(a => a.CajaRegistradora)
            .WithMany(c => c.Arqueos)
            .HasForeignKey(a => a.CajaRegistradoraId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ArqueoCaja>()
            .HasOne(a => a.Usuario)
            .WithMany(u => u.ArqueosCaja)
            .HasForeignKey(a => a.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // DetalleArqueoCaja
        modelBuilder.Entity<DetalleArqueoCaja>()
            .HasKey(d => d.Id);
        modelBuilder.Entity<DetalleArqueoCaja>()
            .HasOne(d => d.ArqueoCaja)
            .WithMany(a => a.Detalles)
            .HasForeignKey(d => d.ArqueoCajaId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<DetalleArqueoCaja>()
            .HasOne(d => d.MetodoPago)
            .WithMany(m => m.DetallesArqueo)
            .HasForeignKey(d => d.MetodoPagoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Devolucion
        modelBuilder.Entity<Devolucion>()
            .HasKey(d => d.Id);
        modelBuilder.Entity<Devolucion>()
            .HasOne(d => d.Venta)
            .WithMany(v => v.Devoluciones)
            .HasForeignKey(d => d.VentaId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Devolucion>()
            .HasOne(d => d.Usuario)
            .WithMany(u => u.Devoluciones)
            .HasForeignKey(d => d.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // DetalleDevolucion
        modelBuilder.Entity<DetalleDevolucion>()
            .HasKey(d => d.Id);
        modelBuilder.Entity<DetalleDevolucion>()
            .HasOne(d => d.Devolucion)
            .WithMany(d => d.Detalles)
            .HasForeignKey(d => d.DevolcionId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<DetalleDevolucion>()
            .HasOne(d => d.Producto)
            .WithMany(p => p.DetallesDevoluciones)
            .HasForeignKey(d => d.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        // AuditoriaLog
        modelBuilder.Entity<AuditoriaLog>()
            .HasKey(a => a.Id);
        modelBuilder.Entity<AuditoriaLog>()
            .HasOne(a => a.Usuario)
            .WithMany(u => u.AuditLogs)
            .HasForeignKey(a => a.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // ModuloAcceso
        modelBuilder.Entity<ModuloAcceso>()
            .HasKey(m => m.Id);
        modelBuilder.Entity<ModuloAcceso>()
            .HasOne(m => m.Usuario)
            .WithMany(u => u.ModuloAccesos)
            .HasForeignKey(m => m.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}