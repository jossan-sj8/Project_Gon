using Project_Gon.Core.Entities;

namespace Project_Gon.Infrastructure.Repositories;

public interface IUnitOfWork : IAsyncDisposable
{
    IRepository<Empresa> Empresas { get; }
    IRepository<Sucursal> Sucursales { get; }
    IRepository<Usuario> Usuarios { get; }
    IRepository<Categoria> Categorias { get; }
    IRepository<Producto> Productos { get; }
    IRepository<Stock> Stocks { get; }
    IRepository<MovimientoStock> MovimientosStock { get; }
    IRepository<Cliente> Clientes { get; }
    IRepository<MetodoPago> MetodosPago { get; }
    IRepository<Venta> Ventas { get; }
    IRepository<DetalleVenta> DetallesVenta { get; }
    IRepository<Pago> Pagos { get; }
    IRepository<Proveedor> Proveedores { get; }
    IRepository<PrecioProveedor> PreciosProveedor { get; }
    IRepository<CajaRegistradora> CajasRegistradoras { get; }
    IRepository<ArqueoCaja> ArqueosCaja { get; }
    IRepository<DetalleArqueoCaja> DetallesArqueoCaja { get; }
    IRepository<Devolucion> Devoluciones { get; }
    IRepository<DetalleDevolucion> DetallesDevoluciones { get; }
    IRepository<AuditoriaLog> AuditoriaLogs { get; }
    IRepository<ModuloAcceso> ModulosAcceso { get; }

    Task<int> SaveChangesAsync();
    Task RollbackAsync();
}