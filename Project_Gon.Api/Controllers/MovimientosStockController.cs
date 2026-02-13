namespace Project_Gon.Api.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Gon.Core.DTOs.MovimientoStock;
using Project_Gon.Core.Entities;
using Project_Gon.Core.Enums;
using Project_Gon.Infrastructure.Repositories;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MovimientosStockController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<MovimientosStockController> _logger;

    public MovimientosStockController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MovimientosStockController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: api/movimientosstock
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovimientoStockDto>>> GetMovimientos(
        [FromQuery] int? stockId = null,
        [FromQuery] TipoMovimientoStock? tipo = null,
        [FromQuery] DateTime? fechaInicio = null,
        [FromQuery] DateTime? fechaFin = null)
    {
        try
        {
            var movimientos = await _unitOfWork.MovimientosStock.GetAllAsync(
                include: q => q
                    .Include(m => m.Stock)
                        .ThenInclude(s => s.Producto)
                    .Include(m => m.Stock)
                        .ThenInclude(s => s.Sucursal)
                    .Include(m => m.Usuario!) 
            );

            // Filtros
            if (stockId.HasValue)
                movimientos = movimientos.Where(m => m.StockId == stockId.Value).ToList();

            if (tipo.HasValue)
                movimientos = movimientos.Where(m => m.Tipo == tipo.Value).ToList();

            if (fechaInicio.HasValue)
                movimientos = movimientos.Where(m => m.CreatedAt >= fechaInicio.Value).ToList();

            if (fechaFin.HasValue)
                movimientos = movimientos.Where(m => m.CreatedAt <= fechaFin.Value).ToList();

            var movimientosDto = _mapper.Map<IEnumerable<MovimientoStockDto>>(movimientos);
            return Ok(movimientosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener movimientos de stock");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // GET: api/movimientosstock/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<MovimientoStockDto>> GetMovimiento(int id)
    {
        try
        {
            var movimiento = await _unitOfWork.MovimientosStock.GetByIdAsync(
                id,
                include: q => q
                    .Include(m => m.Stock)
                        .ThenInclude(s => s.Producto)
                    .Include(m => m.Stock)
                        .ThenInclude(s => s.Sucursal)
                    .Include(m => m.Usuario!)  // ✅ Agregar !
            );

            if (movimiento == null)
            {
                _logger.LogWarning("Movimiento de stock con ID {MovimientoId} no encontrado", id);
                return NotFound($"Movimiento de stock con ID {id} no encontrado");
            }

            var movimientoDto = _mapper.Map<MovimientoStockDto>(movimiento);
            return Ok(movimientoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener movimiento de stock {MovimientoId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // POST: api/movimientosstock
    [HttpPost]
    [Authorize(Roles = "AdminGlobal,AdminEmpresa,AdminSucursal")]
    public async Task<ActionResult<MovimientoStockDto>> CreateMovimiento(CreateMovimientoStockDto createDto)
    {
        try
        {
            // Validar que el stock existe
            var stock = await _unitOfWork.Stocks.GetByIdAsync(
                createDto.StockId,
                include: q => q.Include(s => s.Producto).Include(s => s.Sucursal)
            );

            if (stock == null)
            {
                _logger.LogWarning("Stock con ID {StockId} no encontrado", createDto.StockId);
                return NotFound($"Stock con ID {createDto.StockId} no encontrado");
            }

            // Validar stock disponible para salidas
            if (createDto.Tipo == TipoMovimientoStock.Salida && stock.Cantidad < createDto.Cantidad)
            {
                _logger.LogWarning("Stock insuficiente. Disponible: {Disponible}, Solicitado: {Solicitado}",
                    stock.Cantidad, createDto.Cantidad);
                return BadRequest($"Stock insuficiente. Disponible: {stock.Cantidad}, Solicitado: {createDto.Cantidad}");
            }

            // Actualizar cantidad del stock
            if (createDto.Tipo == TipoMovimientoStock.Entrada)
                stock.Cantidad += createDto.Cantidad;
            else if (createDto.Tipo == TipoMovimientoStock.Salida)
                stock.Cantidad -= createDto.Cantidad;

            stock.UpdatedAt = DateTime.UtcNow;

            var movimiento = _mapper.Map<MovimientoStock>(createDto);
            movimiento.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.MovimientosStock.AddAsync(movimiento);
            await _unitOfWork.Stocks.UpdateAsync(stock);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Movimiento de stock creado: ID {MovimientoId}, Tipo {Tipo}, Cantidad {Cantidad}",
                movimiento.Id, movimiento.Tipo, movimiento.Cantidad);

            var movimientoCreado = await _unitOfWork.MovimientosStock.GetByIdAsync(
                movimiento.Id,
                include: q => q
                    .Include(m => m.Stock)
                        .ThenInclude(s => s.Producto)
                    .Include(m => m.Stock)
                        .ThenInclude(s => s.Sucursal)
                    .Include(m => m.Usuario!)  
            );

            var movimientoDto = _mapper.Map<MovimientoStockDto>(movimientoCreado);
            return CreatedAtAction(nameof(GetMovimiento), new { id = movimiento.Id }, movimientoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear movimiento de stock");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // GET: api/movimientosstock/producto/{productoId}
    [HttpGet("producto/{productoId}")]
    public async Task<ActionResult<IEnumerable<MovimientoStockDto>>> GetMovimientosByProducto(int productoId)
    {
        try
        {
            var movimientos = await _unitOfWork.MovimientosStock.GetAllAsync(
                predicate: m => m.Stock.ProductoId == productoId,
                include: q => q
                    .Include(m => m.Stock)
                        .ThenInclude(s => s.Producto)
                    .Include(m => m.Stock)
                        .ThenInclude(s => s.Sucursal)
                    .Include(m => m.Usuario!)  
            );

            var movimientosDto = _mapper.Map<IEnumerable<MovimientoStockDto>>(movimientos);
            return Ok(movimientosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener movimientos del producto {ProductoId}", productoId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // GET: api/movimientosstock/sucursal/{sucursalId}
    [HttpGet("sucursal/{sucursalId}")]
    public async Task<ActionResult<IEnumerable<MovimientoStockDto>>> GetMovimientosBySucursal(int sucursalId)
    {
        try
        {
            var movimientos = await _unitOfWork.MovimientosStock.GetAllAsync(
                predicate: m => m.Stock.SucursalId == sucursalId,
                include: q => q
                    .Include(m => m.Stock)
                        .ThenInclude(s => s.Producto)
                    .Include(m => m.Stock)
                        .ThenInclude(s => s.Sucursal)
                    .Include(m => m.Usuario!)  // ✅ Agregar !
            );

            var movimientosDto = _mapper.Map<IEnumerable<MovimientoStockDto>>(movimientos);
            return Ok(movimientosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener movimientos de la sucursal {SucursalId}", sucursalId);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}