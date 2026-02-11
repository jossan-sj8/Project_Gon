using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Project_Gon.Api.Extensions;
using Project_Gon.Core.DTOs.Venta;
using Project_Gon.Core.Entities;
using Project_Gon.Core.Enums;
using Project_Gon.Infrastructure.Repositories;
using System.Linq.Expressions;

namespace Project_Gon.Api.Controllers;

/// <summary>
/// Controlador para gestionar ventas.
/// Incluye lógica de negocio: validación de stock, cálculo de totales, descuento automático de inventario.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VentasController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<VentasController> _logger;

    // IVA 19% (configurable)
    private const decimal IVA_PORCENTAJE = 0.19m;

    public VentasController(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<VentasController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: api/ventas
    /// <summary>
    /// Obtiene todas las ventas según el rol del usuario autenticado.
    /// - AdminGlobal: todas las ventas de todas las empresas
    /// - Administrador: solo ventas de su empresa
    /// - Vendedor: solo ventas de su sucursal
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VentaDto>>> GetAll(
        [FromQuery] int? sucursalId = null,
        [FromQuery] EstadoVenta? estado = null,
        [FromQuery] DateTime? fechaDesde = null,
        [FromQuery] DateTime? fechaHasta = null)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();
            var userSucursalId = User.GetSucursalId();

            Expression<Func<Venta, bool>>? predicate = null;

            if (userRole == "AdminGlobal")
            {
                // Sin filtro, puede ver todo
                predicate = v => true;
            }
            else if (userRole == "Administrador")
            {
                // Solo ventas de su empresa
                predicate = v => v.EmpresaId == empresaId;
            }
            else // Vendedor
            {
                // Solo ventas de su sucursal
                predicate = v => v.EmpresaId == empresaId && v.SucursalId == userSucursalId;
            }

            // Aplicar filtros adicionales
            if (sucursalId.HasValue)
            {
                var prevPredicate = predicate;
                predicate = v => prevPredicate.Compile()(v) && v.SucursalId == sucursalId.Value;
            }

            if (estado.HasValue)
            {
                var prevPredicate = predicate;
                predicate = v => prevPredicate.Compile()(v) && v.Estado == estado.Value;
            }

            if (fechaDesde.HasValue)
            {
                var prevPredicate = predicate;
                predicate = v => prevPredicate.Compile()(v) && v.CreatedAt >= fechaDesde.Value;
            }

            if (fechaHasta.HasValue)
            {
                var prevPredicate = predicate;
                var fechaHastaFin = fechaHasta.Value.Date.AddDays(1).AddTicks(-1);
                predicate = v => prevPredicate.Compile()(v) && v.CreatedAt <= fechaHastaFin;
            }

            var ventas = await _unitOfWork.Ventas.GetAllAsync(
                predicate: predicate,
                include: query => query
                    .Include(v => v.Empresa)
                    .Include(v => v.Sucursal)
                    .Include(v => v.Cliente)
                    .Include(v => v.Usuario)
                    .Include(v => v.Detalles)
                        .ThenInclude(d => d.Producto)
            );

            var ventasDto = _mapper.Map<IEnumerable<VentaDto>>(ventas);

            _logger.LogInformation(
                "Retrieved {Count} ventas for user {UserId} with role {Role}",
                ventasDto.Count(),
                User.GetUserId(),
                userRole
            );

            return Ok(ventasDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener ventas");
            return StatusCode(500, "Error al obtener ventas");
        }
    }

    // GET: api/ventas/{id}
    /// <summary>
    /// Obtiene una venta específica por su ID con validación de acceso.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<VentaDto>> GetById(int id)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();
            var sucursalId = User.GetSucursalId();

            var venta = await _unitOfWork.Ventas.GetByIdAsync(
                id,
                include: query => query
                    .Include(v => v.Empresa)
                    .Include(v => v.Sucursal)
                    .Include(v => v.Cliente)
                    .Include(v => v.Usuario)
                    .Include(v => v.Detalles)
                        .ThenInclude(d => d.Producto)
            );

            if (venta == null)
            {
                _logger.LogWarning("Venta con ID {Id} no encontrada", id);
                return NotFound($"Venta con ID {id} no encontrada");
            }

            // Validar acceso según rol
            if (userRole == "Vendedor" && (venta.EmpresaId != empresaId || venta.SucursalId != sucursalId))
            {
                return Forbid("No tienes acceso a esta venta");
            }
            else if (userRole == "Administrador" && venta.EmpresaId != empresaId)
            {
                return Forbid("No tienes acceso a esta venta");
            }

            var ventaDto = _mapper.Map<VentaDto>(venta);
            return Ok(ventaDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener venta {Id}", id);
            return StatusCode(500, "Error al obtener venta");
        }
    }

    // POST: api/ventas
    /// <summary>
    /// Crea una nueva venta con los siguientes pasos:
    /// 1. Validar productos y stock disponible
    /// 2. Calcular subtotales, IVA y total
    /// 3. Descontar stock automáticamente
    /// 4. Registrar movimientos de stock
    /// 5. Guardar venta con transacción
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<VentaDto>> Create([FromBody] CreateVentaDto createDto)
    {
       

        try
        {
            var userId = User.GetUserId();
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();
            var userSucursalId = User.GetSucursalId();

            // ===== VALIDACIÓN 1: Empresa =====
            if (userRole != "AdminGlobal" && createDto.EmpresaId != empresaId)
            {
                return Forbid("No puedes crear ventas para otras empresas");
            }

            // ===== VALIDACIÓN 2: Sucursal =====
            if (userRole == "Vendedor" && createDto.SucursalId != userSucursalId)
            {
                return Forbid("Solo puedes crear ventas en tu sucursal");
            }

            var sucursal = await _unitOfWork.Sucursales.GetByIdAsync(createDto.SucursalId);
            if (sucursal == null)
            {
                return BadRequest($"La sucursal con ID {createDto.SucursalId} no existe");
            }

            if (sucursal.EmpresaId != createDto.EmpresaId)
            {
                return BadRequest("La sucursal no pertenece a la empresa especificada");
            }

            // ===== VALIDACIÓN 3: Cliente (si se proporciona) =====
            if (createDto.ClienteId.HasValue)
            {
                var cliente = await _unitOfWork.Clientes.GetByIdAsync(createDto.ClienteId.Value);
                if (cliente == null)
                {
                    return BadRequest($"El cliente con ID {createDto.ClienteId} no existe");
                }

                if (cliente.EmpresaId != createDto.EmpresaId)
                {
                    return BadRequest("El cliente no pertenece a la empresa especificada");
                }
            }

            // ===== VALIDACIÓN 4: Productos y Stock =====
            var detallesConDatos = new List<(CreateDetalleVentaDto Dto, Producto Producto, Stock Stock)>();
            decimal subtotalTotal = 0;

            foreach (var detalle in createDto.Detalles)
            {
                // Obtener producto
                var producto = await _unitOfWork.Productos.GetByIdAsync(detalle.ProductoId);
                if (producto == null)
                {
                    return BadRequest($"El producto con ID {detalle.ProductoId} no existe");
                }

                if (producto.EmpresaId != createDto.EmpresaId)
                {
                    return BadRequest($"El producto '{producto.Nombre}' no pertenece a la empresa especificada");
                }

                // Obtener stock
                var stock = await _unitOfWork.Stocks.GetAsync(
                    s => s.ProductoId == detalle.ProductoId && s.SucursalId == createDto.SucursalId
                );

                if (stock == null)
                {
                    return BadRequest($"No hay stock del producto '{producto.Nombre}' en esta sucursal");
                }

                // Validar cantidad disponible
                if (stock.Cantidad < detalle.Cantidad)
                {
                    return BadRequest(
                        $"Stock insuficiente del producto '{producto.Nombre}'. " +
                        $"Disponible: {stock.Cantidad}, Solicitado: {detalle.Cantidad}"
                    );
                }

                detallesConDatos.Add((detalle, producto, stock));

                // Calcular subtotal del detalle (usar Precio en lugar de PrecioVenta)
                var precioUnitario = detalle.PrecioUnitario ?? producto.Precio; // ✅ CORREGIDO
                subtotalTotal += precioUnitario * detalle.Cantidad;
            }

            // ===== CÁLCULO DE TOTALES =====
            var iva = subtotalTotal * IVA_PORCENTAJE;
            var total = subtotalTotal + iva;

            // ===== CREAR VENTA =====
            var venta = _mapper.Map<Venta>(createDto);
            venta.UsuarioId = userId;
            venta.SubTotal = subtotalTotal;
            venta.Iva = iva;
            venta.Total = total;
            venta.Estado = EstadoVenta.Completada;
            venta.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Ventas.AddAsync(venta);
            await _unitOfWork.SaveChangesAsync(); // Necesario para obtener venta.Id

            // ===== CREAR DETALLES Y ACTUALIZAR STOCK =====
            foreach (var (dto, producto, stock) in detallesConDatos)
            {
                var precioUnitario = dto.PrecioUnitario ?? producto.Precio; // ✅ CORREGIDO

                // Crear detalle de venta
                var detalleVenta = new DetalleVenta
                {
                    VentaId = venta.Id,
                    ProductoId = dto.ProductoId,
                    Cantidad = dto.Cantidad,
                    PrecioUnitario = precioUnitario,
                    Subtotal = precioUnitario * dto.Cantidad,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.DetallesVenta.AddAsync(detalleVenta);

                // Descontar stock
                stock.Cantidad -= dto.Cantidad;
                stock.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Stocks.UpdateAsync(stock);

                // Registrar movimiento de stock
                var movimiento = new MovimientoStock
                {
                    StockId = stock.Id,
                    Tipo = TipoMovimientoStock.Salida, // using Project_Gon.Core.Enums;
                    Cantidad = dto.Cantidad,
                    Motivo = $"Venta #{venta.Id}",
                    UsuarioId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.MovimientosStock.AddAsync(movimiento);

                _logger.LogInformation(
                    "Stock descontado: Producto {ProductoId}, Cantidad {Cantidad}, Stock restante: {StockRestante}",
                    producto.Id, dto.Cantidad, stock.Cantidad
                );
            }

            // Guardar todos los cambios
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Venta creada exitosamente: ID {VentaId}, Total: ${Total}, Usuario: {UserId}",
                venta.Id, venta.Total, userId
            );

            // Obtener venta completa con relaciones
            venta = await _unitOfWork.Ventas.GetByIdAsync(
                venta.Id,
                include: query => query
                    .Include(v => v.Empresa)
                    .Include(v => v.Sucursal)
                    .Include(v => v.Cliente)
                    .Include(v => v.Usuario)
                    .Include(v => v.Detalles)
                        .ThenInclude(d => d.Producto)
            ) ?? throw new InvalidOperationException("Venta no encontrada después de crearla");

            var ventaDto = _mapper.Map<VentaDto>(venta);

            return CreatedAtAction(nameof(GetById), new { id = venta.Id }, ventaDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear venta");
            return StatusCode(500, $"Error al crear venta: {ex.Message}");
        }
    }

    // PUT: api/ventas/{id}
    /// <summary>
    /// Actualiza una venta existente.
    /// IMPORTANTE: Solo se puede cambiar estado y número de comprobante.
    /// Para modificar productos usar devoluciones.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminGlobalOrAdminEmpresa")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateVentaDto updateDto)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            var venta = await _unitOfWork.Ventas.GetByIdAsync(id);

            if (venta == null)
            {
                return NotFound($"Venta con ID {id} no encontrada");
            }

            // Validar acceso
            if (userRole != "AdminGlobal" && venta.EmpresaId != empresaId)
            {
                return Forbid("No tienes acceso a esta venta");
            }

            // Validar cancelación
            if (updateDto.Estado == EstadoVenta.Cancelada && venta.Estado == EstadoVenta.Completada)
            {
                return BadRequest(
                    "No se puede cancelar una venta completada directamente. " +
                    "Use el endpoint de devoluciones para revertir el stock."
                );
            }

            _mapper.Map(updateDto, venta);

            await _unitOfWork.Ventas.UpdateAsync(venta);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Venta actualizada: ID {Id}, Estado: {Estado}", id, venta.Estado);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar venta {Id}", id);
            return StatusCode(500, "Error al actualizar venta");
        }
    }

    // DELETE: api/ventas/{id}
    /// <summary>
    /// Elimina una venta. Solo AdminGlobal y Administrador.
    /// IMPORTANTE: Esto es destructivo y NO devuelve el stock.
    /// Usar con precaución. Recomendado: cancelar en lugar de eliminar.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminGlobalOrAdminEmpresa")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            var venta = await _unitOfWork.Ventas.GetByIdAsync(
                id,
                include: query => query
                    .Include(v => v.Detalles)
                    .Include(v => v.Pagos)
                    .Include(v => v.Devoluciones)
            );

            if (venta == null)
            {
                return NotFound($"Venta con ID {id} no encontrada");
            }

            // Validar acceso
            if (userRole != "AdminGlobal" && venta.EmpresaId != empresaId)
            {
                return Forbid("No tienes acceso a esta venta");
            }

            // Validar que no existan pagos
            if (venta.Pagos.Count > 0)
            {
                return BadRequest(
                    "No se puede eliminar una venta que tiene pagos asociados. " +
                    "Por favor, elimina los pagos primero o cancela la venta."
                );
            }

            // Validar que no existan devoluciones
            if (venta.Devoluciones.Count > 0)
            {
                return BadRequest(
                    "No se puede eliminar una venta que tiene devoluciones asociadas."
                );
            }

            await _unitOfWork.Ventas.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogWarning("Venta ELIMINADA (destructivo): ID {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar venta {Id}", id);
            return StatusCode(500, "Error al eliminar venta");
        }
    }
}