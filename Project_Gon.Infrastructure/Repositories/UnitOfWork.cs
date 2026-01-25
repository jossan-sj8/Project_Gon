using Project_Gon.Core.Entities;
using Project_Gon.Infrastructure.Data;

namespace Project_Gon.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    private IRepository<Empresa>? _empresas;
    private IRepository<Sucursal>? _sucursales;
    private IRepository<Usuario>? _usuarios;
    private IRepository<Categoria>? _categorias;
    private IRepository<Producto>? _productos;
    private IRepository<Stock>? _stocks;
    private IRepository<MovimientoStock>? _movimientosStock;
    private IRepository<Cliente>? _clientes;
    private IRepository<MetodoPago>? _metodosPago;
    private IRepository<Venta>? _ventas;
    private IRepository<DetalleVenta>? _detallesVenta;
    private IRepository<Pago>? _pagos;
    private IRepository<Proveedor>? _proveedores;
    private IRepository<PrecioProveedor>? _preciosProveedor;
    private IRepository<CajaRegistradora>? _cajasRegistradoras;
    private IRepository<ArqueoCaja>? _arqueosCaja;
    private IRepository<DetalleArqueoCaja>? _detallesArqueoCaja;
    private IRepository<Devolucion>? _devoluciones;
    private IRepository<DetalleDevolucion>? _detallesDevoluciones;
    private IRepository<AuditoriaLog>? _auditoriaLogs;
    private IRepository<ModuloAcceso>? _modulosAcceso;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IRepository<Empresa> Empresas => _empresas ??= new Repository<Empresa>(_context);
    public IRepository<Sucursal> Sucursales => _sucursales ??= new Repository<Sucursal>(_context);
    public IRepository<Usuario> Usuarios => _usuarios ??= new Repository<Usuario>(_context);
    public IRepository<Categoria> Categorias => _categorias ??= new Repository<Categoria>(_context);
    public IRepository<Producto> Productos => _productos ??= new Repository<Producto>(_context);
    public IRepository<Stock> Stocks => _stocks ??= new Repository<Stock>(_context);
    public IRepository<MovimientoStock> MovimientosStock => _movimientosStock ??= new Repository<MovimientoStock>(_context);
    public IRepository<Cliente> Clientes => _clientes ??= new Repository<Cliente>(_context);
    public IRepository<MetodoPago> MetodosPago => _metodosPago ??= new Repository<MetodoPago>(_context);
    public IRepository<Venta> Ventas => _ventas ??= new Repository<Venta>(_context);
    public IRepository<DetalleVenta> DetallesVenta => _detallesVenta ??= new Repository<DetalleVenta>(_context);
    public IRepository<Pago> Pagos => _pagos ??= new Repository<Pago>(_context);
    public IRepository<Proveedor> Proveedores => _proveedores ??= new Repository<Proveedor>(_context);
    public IRepository<PrecioProveedor> PreciosProveedor => _preciosProveedor ??= new Repository<PrecioProveedor>(_context);
    public IRepository<CajaRegistradora> CajasRegistradoras => _cajasRegistradoras ??= new Repository<CajaRegistradora>(_context);
    public IRepository<ArqueoCaja> ArqueosCaja => _arqueosCaja ??= new Repository<ArqueoCaja>(_context);
    public IRepository<DetalleArqueoCaja> DetallesArqueoCaja => _detallesArqueoCaja ??= new Repository<DetalleArqueoCaja>(_context);
    public IRepository<Devolucion> Devoluciones => _devoluciones ??= new Repository<Devolucion>(_context);
    public IRepository<DetalleDevolucion> DetallesDevoluciones => _detallesDevoluciones ??= new Repository<DetalleDevolucion>(_context);
    public IRepository<AuditoriaLog> AuditoriaLogs => _auditoriaLogs ??= new Repository<AuditoriaLog>(_context);
    public IRepository<ModuloAcceso> ModulosAcceso => _modulosAcceso ??= new Repository<ModuloAcceso>(_context);

    public async Task<int> SaveChangesAsync()
    {
        try
        {
            return await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error al guardar cambios en la base de datos.", ex);
        }
    }

    public async Task RollbackAsync()
    {
        await _context.DisposeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}