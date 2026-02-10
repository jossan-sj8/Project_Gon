using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Gon.Api.Extensions;
using Project_Gon.Core.DTOs.Stock;
using Project_Gon.Core.Entities;
using Project_Gon.Infrastructure.Repositories;
using System.Linq.Expressions;

namespace Project_Gon.Api.Controllers;

/// <summary>
/// Controlador para gestionar stock de productos en sucursales.
/// Endpoints CRUD con autorización basada en roles (JWT).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StocksController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<StocksController> _logger;

    public StocksController(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<StocksController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: api/stocks
    /// <summary>
    /// Obtiene todos los stocks según el rol del usuario autenticado.
    /// - AdminGlobal: todos los stocks de todas las empresas
    /// - AdminEmpresa/Vendedor: solo stocks de su empresa
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockDto>>> GetAll()
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            Expression<Func<Stock, bool>>? predicate = null;

            if (userRole != "AdminGlobal")
            {
                // Filtrar stocks de la empresa del usuario
                predicate = s => s.Producto.EmpresaId == empresaId;
            }

            var stocks = await _unitOfWork.Stocks.GetAllAsync(
                predicate: predicate,
                include: query => query
                    .Include(s => s.Producto)
                    .Include(s => s.Sucursal)
            );

            var stocksDto = _mapper.Map<IEnumerable<StockDto>>(stocks);

            _logger.LogInformation(
                "Retrieved {Count} stocks for user {UserId} with role {Role}",
                stocksDto.Count(),
                User.GetUserId(),
                userRole
            );

            return Ok(stocksDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener stocks");
            return StatusCode(500, "Error al obtener stocks");
        }
    }

    // GET: api/stocks/{id}
    /// <summary>
    /// Obtiene un stock específico por su ID con validación de acceso.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<StockDto>> GetById(int id)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            var stock = await _unitOfWork.Stocks.GetByIdAsync(
                id,
                include: query => query
                    .Include(s => s.Producto)
                    .Include(s => s.Sucursal)
            );

            if (stock == null)
            {
                _logger.LogWarning("Stock con ID {Id} no encontrado", id);
                return NotFound($"Stock con ID {id} no encontrado");
            }

            // Validar acceso: AdminGlobal puede ver todos, otros solo su empresa
            if (userRole != "AdminGlobal" && stock.Producto.EmpresaId != empresaId)
            {
                return Forbid("No tienes acceso a este stock");
            }

            var stockDto = _mapper.Map<StockDto>(stock);
            return Ok(stockDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener stock {Id}", id);
            return StatusCode(500, "Error al obtener stock");
        }
    }

    // POST: api/stocks
    /// <summary>
    /// Crea un nuevo registro de stock. Solo AdminGlobal y Administrador.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AdminGlobalOrAdminEmpresa")]
    public async Task<ActionResult<StockDto>> Create([FromBody] CreateStockDto createDto)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            // Validar que el producto exista y pertenezca a la empresa del usuario
            var producto = await _unitOfWork.Productos.GetByIdAsync(createDto.ProductoId);
            if (producto == null)
            {
                return BadRequest($"El producto con ID {createDto.ProductoId} no existe");
            }

            if (userRole != "AdminGlobal" && producto.EmpresaId != empresaId)
            {
                return Forbid("No puedes crear stock para productos de otras empresas");
            }

            // Validar que la sucursal exista y pertenezca a la empresa del usuario
            var sucursal = await _unitOfWork.Sucursales.GetByIdAsync(createDto.SucursalId);
            if (sucursal == null)
            {
                return BadRequest($"La sucursal con ID {createDto.SucursalId} no existe");
            }

            if (userRole != "AdminGlobal" && sucursal.EmpresaId != empresaId)
            {
                return Forbid("No puedes crear stock en sucursales de otras empresas");
            }

            // Validar que no exista ya un stock para este producto-sucursal
            var stockExistente = await _unitOfWork.Stocks.GetAsync(
                s => s.ProductoId == createDto.ProductoId && s.SucursalId == createDto.SucursalId
            );

            if (stockExistente != null)
            {
                return BadRequest(
                    $"Ya existe un stock para el producto {producto.Nombre} en la sucursal {sucursal.Nombre}"
                );
            }

            var stock = _mapper.Map<Stock>(createDto);

            await _unitOfWork.Stocks.AddAsync(stock);
            await _unitOfWork.SaveChangesAsync();

            // Obtener el stock creado con sus relaciones incluidas
            stock = await _unitOfWork.Stocks.GetByIdAsync(
                stock.Id,
                include: query => query
                    .Include(s => s.Producto)
                    .Include(s => s.Sucursal)
            ) ?? throw new InvalidOperationException("Stock no encontrado después de crearlo");

            var stockDto = _mapper.Map<StockDto>(stock);

            _logger.LogInformation(
                "Stock creado: Producto {ProductoId}, Sucursal {SucursalId}, Cantidad {Cantidad}",
                stock.ProductoId, stock.SucursalId, stock.Cantidad
            );

            return CreatedAtAction(nameof(GetById), new { id = stock.Id }, stockDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear stock");
            return StatusCode(500, "Error al crear stock");
        }
    }

    // PUT: api/stocks/{id}
    /// <summary>
    /// Actualiza un stock existente. Solo AdminGlobal y Administrador.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminGlobalOrAdminEmpresa")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStockDto updateDto)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            var stock = await _unitOfWork.Stocks.GetByIdAsync(
                id,
                include: query => query
                    .Include(s => s.Producto)
                    .Include(s => s.Sucursal)
            );

            if (stock == null)
            {
                return NotFound($"Stock con ID {id} no encontrado");
            }

            // Validar acceso
            if (userRole != "AdminGlobal" && stock.Producto.EmpresaId != empresaId)
            {
                return Forbid("No tienes acceso a este stock");
            }

            _mapper.Map(updateDto, stock);

            await _unitOfWork.Stocks.UpdateAsync(stock);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Stock actualizado: ID {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar stock {Id}", id);
            return StatusCode(500, "Error al actualizar stock");
        }
    }

    // DELETE: api/stocks/{id}
    /// <summary>
    /// Elimina un stock. Solo AdminGlobal y Administrador.
    /// Nota: Solo puede eliminarse stock si no tiene movimientos asociados.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminGlobalOrAdminEmpresa")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            var stock = await _unitOfWork.Stocks.GetByIdAsync(
                id,
                include: query => query
                    .Include(s => s.Producto)
                    .Include(s => s.Movimientos)
            );

            if (stock == null)
            {
                return NotFound($"Stock con ID {id} no encontrado");
            }

            // Validar acceso
            if (userRole != "AdminGlobal" && stock.Producto.EmpresaId != empresaId)
            {
                return Forbid("No tienes acceso a este stock");
            }

            // Validar que no existan movimientos asociados
            if (stock.Movimientos.Count > 0)
            {
                return BadRequest(
                    "No se puede eliminar un stock que tiene movimientos asociados. " +
                    "Por favor, elimina los movimientos primero."
                );
            }

            await _unitOfWork.Stocks.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Stock eliminado: ID {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar stock {Id}", id);
            return StatusCode(500, "Error al eliminar stock");
        }
    }
}