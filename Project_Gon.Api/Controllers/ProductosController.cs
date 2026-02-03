using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Gon.Core.DTOs.Producto;
using Project_Gon.Core.Entities;
using Project_Gon.Infrastructure.Repositories;
using Project_Gon.Api.Extensions;
using System.Linq.Expressions;

namespace Project_Gon.Api.Controllers;

/// <summary>
/// Controlador para gestionar productos del sistema.
/// Endpoints CRUD con autorización basada en roles (JWT).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductosController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductosController> _logger;

    public ProductosController(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ProductosController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: api/productos
    /// <summary>
    /// Obtiene todos los productos según el rol del usuario autenticado.
    /// - AdminGlobal: todos los productos
    /// - AdminEmpresa/Vendedor: solo productos de su empresa
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductoDto>>> GetAll()
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            Expression<Func<Producto, bool>>? predicate = null;

            if (userRole == "AdminGlobal")
            {
                predicate = null; // VE TODO
            }
            else
            {
                // AdminEmpresa y Vendedor: solo su empresa
                predicate = p => p.EmpresaId == empresaId;
            }

            var productos = await _unitOfWork.Productos.GetAllAsync(
                predicate: predicate,
                include: query => query
                    .Include(p => p.Empresa)
                    .Include(p => p.Categoria)!
            );

            var productosDto = _mapper.Map<IEnumerable<ProductoDto>>(productos);

            _logger.LogInformation(
                "Retrieved {Count} productos for user {UserId} with role {Role}",
                productosDto.Count(),
                User.GetUserId(),
                userRole
            );

            return Ok(productosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos");
            return StatusCode(500, "Error al obtener productos");
        }
    }

    // GET: api/productos/{id}
    /// <summary>
    /// Obtiene un producto por su ID con validación de acceso.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductoDto>> GetById(int id)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            var producto = await _unitOfWork.Productos.GetByIdAsync(
                id,
                include: query => query
                    .Include(p => p.Empresa)
                    .Include(p => p.Categoria)!
            );

            if (producto == null)
            {
                _logger.LogWarning("Producto con ID {Id} no encontrado", id);
                return NotFound($"Producto con ID {id} no encontrado");
            }

            // Validar acceso según rol
            if (userRole != "AdminGlobal" && producto.EmpresaId != empresaId)
            {
                return Forbid("No tienes acceso a este producto");
            }

            var productoDto = _mapper.Map<ProductoDto>(producto);
            return Ok(productoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener producto {Id}", id);
            return StatusCode(500, "Error al obtener producto");
        }
    }

    // GET: api/productos/categoria/{categoriaId}
    /// <summary>
    /// Obtiene productos por categoría.
    /// </summary>
    [HttpGet("categoria/{categoriaId}")]
    public async Task<ActionResult<IEnumerable<ProductoDto>>> GetByCategoria(int categoriaId)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            Expression<Func<Producto, bool>> predicate;

            if (userRole == "AdminGlobal")
            {
                predicate = p => p.CategoriaId == categoriaId;
            }
            else
            {
                predicate = p => p.CategoriaId == categoriaId && p.EmpresaId == empresaId;
            }

            var productos = await _unitOfWork.Productos.GetAllAsync(
                predicate: predicate,
                include: query => query
                    .Include(p => p.Empresa)
                    .Include(p => p.Categoria)!
            );

            var productosDto = _mapper.Map<IEnumerable<ProductoDto>>(productos);
            return Ok(productosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos por categoría");
            return StatusCode(500, "Error al obtener productos");
        }
    }

    // POST: api/productos
    /// <summary>
    /// Crea un nuevo producto. Solo AdminGlobal y AdminEmpresa.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "AdminGlobal,AdminEmpresa")]
    public async Task<ActionResult<ProductoDto>> Create([FromBody] CreateProductoDto createDto)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            // Validar que el usuario crea dentro de su empresa
            if (userRole != "AdminGlobal" && createDto.EmpresaId != empresaId)
            {
                return Forbid("No puedes crear productos en otras empresas");
            }

            var empresa = await _unitOfWork.Empresas.GetByIdAsync(createDto.EmpresaId);
            if (empresa == null)
            {
                return BadRequest($"La empresa con ID {createDto.EmpresaId} no existe");
            }

            var categoria = await _unitOfWork.Categorias.GetByIdAsync(createDto.CategoriaId);
            if (categoria == null)
            {
                return BadRequest($"La categoría con ID {createDto.CategoriaId} no existe");
            }

            var producto = _mapper.Map<Producto>(createDto);

            await _unitOfWork.Productos.AddAsync(producto);
            await _unitOfWork.SaveChangesAsync();

            producto = await _unitOfWork.Productos.GetByIdAsync(
                producto.Id,
                include: query => query
                    .Include(p => p.Empresa)
                    .Include(p => p.Categoria)!
            ) ?? throw new InvalidOperationException("Producto no encontrado después de crearlo");

            var productoDto = _mapper.Map<ProductoDto>(producto);

            _logger.LogInformation("Producto creado: {Nombre} - ID {Id}",
                producto.Nombre, producto.Id);

            return CreatedAtAction(nameof(GetById), new { id = producto.Id }, productoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear producto");
            return StatusCode(500, "Error al crear producto");
        }
    }

    // PUT: api/productos/{id}
    /// <summary>
    /// Actualiza un producto existente. Solo AdminGlobal y AdminEmpresa.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "AdminGlobal,AdminEmpresa")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductoDto updateDto)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            var producto = await _unitOfWork.Productos.GetByIdAsync(
                id,
                include: query => query
                    .Include(p => p.Empresa)
                    .Include(p => p.Categoria)!
            );

            if (producto == null)
            {
                return NotFound($"Producto con ID {id} no encontrado");
            }

            // Validar acceso según rol
            if (userRole != "AdminGlobal" && producto.EmpresaId != empresaId)
            {
                return Forbid("No tienes acceso a este producto");
            }

            if (updateDto.CategoriaId.HasValue)
            {
                var categoria = await _unitOfWork.Categorias.GetByIdAsync(updateDto.CategoriaId.Value);
                if (categoria == null)
                {
                    return BadRequest($"La categoría con ID {updateDto.CategoriaId} no existe");
                }
            }

            _mapper.Map(updateDto, producto);

            await _unitOfWork.Productos.UpdateAsync(producto);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Producto actualizado: ID {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar producto {Id}", id);
            return StatusCode(500, "Error al actualizar producto");
        }
    }

    // DELETE: api/productos/{id}
    /// <summary>
    /// Elimina un producto. Solo AdminGlobal y AdminEmpresa.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "AdminGlobal,AdminEmpresa")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            var producto = await _unitOfWork.Productos.GetByIdAsync(id);

            if (producto == null)
            {
                return NotFound($"Producto con ID {id} no encontrado");
            }

            // Validar acceso según rol
            if (userRole != "AdminGlobal" && producto.EmpresaId != empresaId)
            {
                return Forbid("No tienes acceso a este producto");
            }

            await _unitOfWork.Productos.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Producto eliminado: ID {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar producto {Id}", id);
            return StatusCode(500, "Error al eliminar producto");
        }
    }
}