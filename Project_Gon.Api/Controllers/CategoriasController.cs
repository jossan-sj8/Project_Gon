using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Gon.Core.DTOs.Categoria;
using Project_Gon.Core.Entities;
using Project_Gon.Infrastructure.Repositories;
using Project_Gon.Api.Extensions;
using System.Linq.Expressions;

namespace Project_Gon.Api.Controllers;

/// <summary>
/// Controlador para gestionar categorías de productos.
/// Endpoints CRUD con autorización basada en roles (JWT).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriasController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CategoriasController> _logger;

    public CategoriasController(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CategoriasController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: api/categorias
    /// <summary>
    /// Obtiene todas las categorías según el rol del usuario autenticado.
    /// - AdminGlobal: todas las categorías
    /// - AdminEmpresa/Vendedor: solo categorías de su empresa
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetAll()
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            Expression<Func<Categoria, bool>>? predicate = null;

            if (userRole == "AdminGlobal")
            {
                predicate = null;
            }
            else
            {
                predicate = c => c.EmpresaId == empresaId;
            }

            var categorias = await _unitOfWork.Categorias.GetAllAsync(
                predicate: predicate,
                include: query => query.Include(c => c.Empresa)
            );

            var categoriasDto = _mapper.Map<IEnumerable<CategoriaDto>>(categorias);

            _logger.LogInformation(
                "Retrieved {Count} categorías for user {UserId} with role {Role}",
                categoriasDto.Count(),
                User.GetUserId(),
                userRole
            );

            return Ok(categoriasDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener categorías");
            return StatusCode(500, "Error al obtener categorías");
        }
    }

    // GET: api/categorias/{id}
    /// <summary>
    /// Obtiene una categoría por su ID con validación de acceso.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoriaDto>> GetById(int id)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            var categoria = await _unitOfWork.Categorias.GetByIdAsync(
                id,
                include: query => query.Include(c => c.Empresa)
            );

            if (categoria == null)
            {
                _logger.LogWarning("Categoría con ID {Id} no encontrada", id);
                return NotFound($"Categoría con ID {id} no encontrada");
            }

            if (userRole != "AdminGlobal" && categoria.EmpresaId != empresaId)
            {
                return Forbid("No tienes acceso a esta categoría");
            }

            var categoriaDto = _mapper.Map<CategoriaDto>(categoria);
            return Ok(categoriaDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener categoría {Id}", id);
            return StatusCode(500, "Error al obtener categoría");
        }
    }

    // POST: api/categorias
    /// <summary>
    /// Crea una nueva categoría. Solo AdminGlobal y AdminEmpresa.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "AdminGlobal,AdminEmpresa")]
    public async Task<ActionResult<CategoriaDto>> Create([FromBody] CreateCategoriaDto createDto)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            if (userRole != "AdminGlobal" && createDto.EmpresaId != empresaId)
            {
                return Forbid("No puedes crear categorías en otras empresas");
            }

            var empresa = await _unitOfWork.Empresas.GetByIdAsync(createDto.EmpresaId);
            if (empresa == null)
            {
                return BadRequest($"La empresa con ID {createDto.EmpresaId} no existe");
            }

            var categoria = _mapper.Map<Categoria>(createDto);

            await _unitOfWork.Categorias.AddAsync(categoria);
            await _unitOfWork.SaveChangesAsync();

            categoria = await _unitOfWork.Categorias.GetByIdAsync(
                categoria.Id,
                include: query => query.Include(c => c.Empresa)
            ) ?? throw new InvalidOperationException("Categoría no encontrada después de crearlo");

            var categoriaDto = _mapper.Map<CategoriaDto>(categoria);

            _logger.LogInformation("Categoría creada: {Nombre} - ID {Id}",
                categoria.Nombre, categoria.Id);

            return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, categoriaDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear categoría");
            return StatusCode(500, "Error al crear categoría");
        }
    }

    // PUT: api/categorias/{id}
    /// <summary>
    /// Actualiza una categoría existente. Solo AdminGlobal y AdminEmpresa.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "AdminGlobal,AdminEmpresa")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoriaDto updateDto)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            var categoria = await _unitOfWork.Categorias.GetByIdAsync(
                id,
                include: query => query.Include(c => c.Empresa)
            );

            if (categoria == null)
            {
                return NotFound($"Categoría con ID {id} no encontrada");
            }

            if (userRole != "AdminGlobal" && categoria.EmpresaId != empresaId)
            {
                return Forbid("No tienes acceso a esta categoría");
            }

            _mapper.Map(updateDto, categoria);

            await _unitOfWork.Categorias.UpdateAsync(categoria);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Categoría actualizada: ID {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar categoría {Id}", id);
            return StatusCode(500, "Error al actualizar categoría");
        }
    }

    // DELETE: api/categorias/{id}
    /// <summary>
    /// Elimina una categoría. Solo AdminGlobal y AdminEmpresa.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "AdminGlobal,AdminEmpresa")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            var categoria = await _unitOfWork.Categorias.GetByIdAsync(id);

            if (categoria == null)
            {
                return NotFound($"Categoría con ID {id} no encontrada");
            }

            if (userRole != "AdminGlobal" && categoria.EmpresaId != empresaId)
            {
                return Forbid("No tienes acceso a esta categoría");
            }

            // Verificar si hay productos con esta categoría
            var productosCount = await _unitOfWork.Productos.CountAsync(
                p => p.CategoriaId == id);

            if (productosCount > 0)
            {
                return BadRequest($"No se puede eliminar: hay {productosCount} productos con esta categoría");
            }

            await _unitOfWork.Categorias.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Categoría eliminada: ID {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar categoría {Id}", id);
            return StatusCode(500, "Error al eliminar categoría");
        }
    }
}