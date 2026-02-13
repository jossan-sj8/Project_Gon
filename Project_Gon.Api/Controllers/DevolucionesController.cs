namespace Project_Gon.Api.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Gon.Core.DTOs.Devolucion;
using Project_Gon.Core.Entities;
using Project_Gon.Core.Enums;
using Project_Gon.Infrastructure.Repositories;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DevolucionesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<DevolucionesController> _logger;

    public DevolucionesController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<DevolucionesController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: api/devoluciones
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DevolucionDto>>> GetDevoluciones(
        [FromQuery] int? ventaId = null,
        [FromQuery] EstadoDevolucion? estado = null)
    {
        try
        {
            var devoluciones = await _unitOfWork.Devoluciones.GetAllAsync(
                include: q => q
                    .Include(d => d.Usuario)
                    .Include(d => d.Detalles)
                        .ThenInclude(dd => dd.Producto)
            );

            if (ventaId.HasValue)
                devoluciones = devoluciones.Where(d => d.VentaId == ventaId.Value).ToList();

            if (estado.HasValue)
                devoluciones = devoluciones.Where(d => d.Estado == estado.Value).ToList();

            var devolucionesDto = _mapper.Map<IEnumerable<DevolucionDto>>(devoluciones);
            return Ok(devolucionesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener devoluciones");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // GET: api/devoluciones/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<DevolucionDto>> GetDevolucion(int id)
    {
        try
        {
            var devolucion = await _unitOfWork.Devoluciones.GetByIdAsync(
                id,
                include: q => q
                    .Include(d => d.Usuario)
                    .Include(d => d.Detalles)
                        .ThenInclude(dd => dd.Producto)
            );

            if (devolucion == null)
            {
                _logger.LogWarning("Devolución con ID {DevolucionId} no encontrada", id);
                return NotFound($"Devolución con ID {id} no encontrada");
            }

            var devolucionDto = _mapper.Map<DevolucionDto>(devolucion);
            return Ok(devolucionDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener devolución {DevolucionId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // POST: api/devoluciones
    [HttpPost]
    [Authorize(Roles = "AdminGlobal,AdminEmpresa,AdminSucursal")]
    public async Task<ActionResult<DevolucionDto>> CreateDevolucion(CreateDevolucionDto createDto)
    {
        try
        {
            // Validar que la venta existe
            var venta = await _unitOfWork.Ventas.GetByIdAsync(
                createDto.VentaId,
                include: q => q.Include(v => v.Detalles)
            );

            if (venta == null)
            {
                _logger.LogWarning("Venta con ID {VentaId} no encontrada", createDto.VentaId);
                return NotFound($"Venta con ID {createDto.VentaId} no encontrada");
            }

            // Validar que el usuario existe
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(createDto.UsuarioId);
            if (usuario == null)
            {
                _logger.LogWarning("Usuario con ID {UsuarioId} no encontrado", createDto.UsuarioId);
                return NotFound($"Usuario con ID {createDto.UsuarioId} no encontrado");
            }

            // Crear la devolución
            var devolucion = _mapper.Map<Devolucion>(createDto);
            devolucion.CreatedAt = DateTime.UtcNow;
            devolucion.Estado = EstadoDevolucion.Pendiente;

            await _unitOfWork.Devoluciones.AddAsync(devolucion);
            await _unitOfWork.SaveChangesAsync();

            // Restaurar stock para cada producto devuelto
            foreach (var detalle in devolucion.Detalles)
            {
                var stock = await _unitOfWork.Stocks.GetAllAsync(
                    predicate: s => s.ProductoId == detalle.ProductoId && s.SucursalId == venta.SucursalId
                );

                var stockItem = stock.FirstOrDefault();
                if (stockItem != null)
                {
                    stockItem.Cantidad += detalle.Cantidad;
                    stockItem.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.Stocks.UpdateAsync(stockItem);

                    // Registrar movimiento de entrada por devolución
                    var movimiento = new MovimientoStock
                    {
                        StockId = stockItem.Id,
                        Cantidad = detalle.Cantidad,
                        Tipo = TipoMovimientoStock.Entrada,
                        Motivo = $"Devolución de venta #{venta.Id}",
                        UsuarioId = createDto.UsuarioId,
                        DevolcionId = devolucion.Id,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.MovimientosStock.AddAsync(movimiento);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Devolución creada exitosamente: ID {DevolucionId}, Venta {VentaId}, Monto ${Monto}",
                devolucion.Id, devolucion.VentaId, devolucion.MontoReembolso);

            var devolucionCreada = await _unitOfWork.Devoluciones.GetByIdAsync(
                devolucion.Id,
                include: q => q
                    .Include(d => d.Usuario)
                    .Include(d => d.Detalles)
                        .ThenInclude(dd => dd.Producto)
            );

            var devolucionDto = _mapper.Map<DevolucionDto>(devolucionCreada);
            return CreatedAtAction(nameof(GetDevolucion), new { id = devolucion.Id }, devolucionDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear devolución");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // PUT: api/devoluciones/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "AdminGlobal,AdminEmpresa,AdminSucursal")]
    public async Task<ActionResult<DevolucionDto>> UpdateDevolucion(int id, UpdateDevolucionDto updateDto)
    {
        try
        {
            var devolucion = await _unitOfWork.Devoluciones.GetByIdAsync(
                id,
                include: q => q
                    .Include(d => d.Usuario)
                    .Include(d => d.Detalles)
                        .ThenInclude(dd => dd.Producto)
            );

            if (devolucion == null)
            {
                _logger.LogWarning("Devolución con ID {DevolucionId} no encontrada", id);
                return NotFound($"Devolución con ID {id} no encontrada");
            }

            // Aplicar cambios
            if (!string.IsNullOrEmpty(updateDto.Motivo))
                devolucion.Motivo = updateDto.Motivo;

            if (updateDto.MontoReembolso.HasValue)
                devolucion.MontoReembolso = updateDto.MontoReembolso.Value;

            if (updateDto.Estado.HasValue)
                devolucion.Estado = updateDto.Estado.Value;

            devolucion.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Devoluciones.UpdateAsync(devolucion);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Devolución actualizada: ID {DevolucionId}", id);

            var devolucionDto = _mapper.Map<DevolucionDto>(devolucion);
            return Ok(devolucionDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar devolución {DevolucionId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // DELETE: api/devoluciones/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "AdminGlobal,AdminEmpresa")]
    public async Task<IActionResult> DeleteDevolucion(int id)
    {
        try
        {
            var devolucion = await _unitOfWork.Devoluciones.GetByIdAsync(id);
            if (devolucion == null)
            {
                _logger.LogWarning("Devolución con ID {DevolucionId} no encontrada", id);
                return NotFound($"Devolución con ID {id} no encontrada");
            }

            await _unitOfWork.Devoluciones.DeleteAsync(devolucion);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Devolución eliminada: ID {DevolucionId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar devolución {DevolucionId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // GET: api/devoluciones/venta/{ventaId}
    [HttpGet("venta/{ventaId}")]
    public async Task<ActionResult<IEnumerable<DevolucionDto>>> GetDevolucionesByVenta(int ventaId)
    {
        try
        {
            var devoluciones = await _unitOfWork.Devoluciones.GetAllAsync(
                predicate: d => d.VentaId == ventaId,
                include: q => q
                    .Include(d => d.Usuario)
                    .Include(d => d.Detalles)
                        .ThenInclude(dd => dd.Producto)
            );

            var devolucionesDto = _mapper.Map<IEnumerable<DevolucionDto>>(devoluciones);
            return Ok(devolucionesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener devoluciones de la venta {VentaId}", ventaId);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}