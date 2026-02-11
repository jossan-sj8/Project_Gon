using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Gon.Api.Extensions;
using Project_Gon.Core.DTOs.Proveedor;
using Project_Gon.Core.Entities;
using Project_Gon.Infrastructure.Repositories;
using System.Linq.Expressions;

namespace Project_Gon.Api.Controllers;

/// <summary>
/// Controlador para gestionar proveedores.
/// Endpoints CRUD con autorización basada en roles (JWT).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProveedoresController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ProveedoresController> _logger;

    public ProveedoresController(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ProveedoresController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: api/proveedores
    /// <summary>
    /// Obtiene todos los proveedores según el rol del usuario autenticado.
    /// - AdminGlobal: todos los proveedores de todas las empresas
    /// - Administrador/Vendedor: solo proveedores de su empresa
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProveedorDto>>> GetAll([FromQuery] bool? activo = null)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            Expression<Func<Proveedor, bool>>? predicate = null;

            if (userRole == "AdminGlobal")
            {
                predicate = activo.HasValue ? p => p.Activo == activo.Value : null;
            }
            else
            {
                predicate = activo.HasValue
                    ? p => p.EmpresaId == empresaId && p.Activo == activo.Value
                    : p => p.EmpresaId == empresaId;
            }

            var proveedores = await _unitOfWork.Proveedores.GetAllAsync(
                predicate: predicate,
                include: query => query.Include(p => p.Empresa)
            );

            var proveedoresDto = _mapper.Map<IEnumerable<ProveedorDto>>(proveedores);

            _logger.LogInformation(
                "Retrieved {Count} proveedores for user {UserId} with role {Role}",
                proveedoresDto.Count(),
                User.GetUserId(),
                userRole
            );

            return Ok(proveedoresDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener proveedores");
            return StatusCode(500, "Error al obtener proveedores");
        }
    }

    // GET: api/proveedores/{id}
    /// <summary>
    /// Obtiene un proveedor específico por su ID con validación de acceso.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProveedorDto>> GetById(int id)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            var proveedor = await _unitOfWork.Proveedores.GetByIdAsync(
                id,
                include: query => query.Include(p => p.Empresa)
            );

            if (proveedor == null)
            {
                _logger.LogWarning("Proveedor con ID {Id} no encontrado", id);
                return NotFound($"Proveedor con ID {id} no encontrado");
            }

            // Validar acceso: AdminGlobal puede ver todos, otros solo su empresa
            if (userRole != "AdminGlobal" && proveedor.EmpresaId != empresaId)
            {
                return Forbid("No tienes acceso a este proveedor");
            }

            var proveedorDto = _mapper.Map<ProveedorDto>(proveedor);
            return Ok(proveedorDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener proveedor {Id}", id);
            return StatusCode(500, "Error al obtener proveedor");
        }
    }

    // POST: api/proveedores
    /// <summary>
    /// Crea un nuevo proveedor. Solo AdminGlobal y Administrador.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AdminGlobalOrAdminEmpresa")]
    public async Task<ActionResult<ProveedorDto>> Create([FromBody] CreateProveedorDto createDto)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            // Validar que el usuario solo cree proveedores para su empresa
            if (userRole != "AdminGlobal" && createDto.EmpresaId != empresaId)
            {
                return Forbid("No puedes crear proveedores para otras empresas");
            }

            // Validar que la empresa existe
            var empresa = await _unitOfWork.Empresas.GetByIdAsync(createDto.EmpresaId);
            if (empresa == null)
            {
                return BadRequest($"La empresa con ID {createDto.EmpresaId} no existe");
            }

            // Validar unicidad de RUT (si se proporciona)
            if (!string.IsNullOrWhiteSpace(createDto.Rut))
            {
                var existingByRut = await _unitOfWork.Proveedores.GetAsync(
                    p => p.Rut == createDto.Rut && p.EmpresaId == createDto.EmpresaId
                );

                if (existingByRut != null)
                {
                    return BadRequest($"Ya existe un proveedor con RUT {createDto.Rut} en esta empresa");
                }
            }

            var proveedor = _mapper.Map<Proveedor>(createDto);

            await _unitOfWork.Proveedores.AddAsync(proveedor);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Proveedor creado: ID {Id}, Nombre {Nombre}, Empresa {EmpresaId}",
                proveedor.Id, proveedor.Nombre, proveedor.EmpresaId
            );

            // Cargar la empresa para el response
            proveedor.Empresa = empresa;

            var proveedorDto = _mapper.Map<ProveedorDto>(proveedor);

            return CreatedAtAction(nameof(GetById), new { id = proveedor.Id }, proveedorDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear proveedor");
            return StatusCode(500, "Error al crear proveedor");
        }
    }

    // PUT: api/proveedores/{id}
    /// <summary>
    /// Actualiza un proveedor existente. Solo AdminGlobal y Administrador.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminGlobalOrAdminEmpresa")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProveedorDto updateDto)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            var proveedor = await _unitOfWork.Proveedores.GetByIdAsync(id);

            if (proveedor == null)
            {
                return NotFound($"Proveedor con ID {id} no encontrado");
            }

            // Validar acceso
            if (userRole != "AdminGlobal" && proveedor.EmpresaId != empresaId)
            {
                return Forbid("No tienes acceso a este proveedor");
            }

            // Validar unicidad de RUT si se está cambiando
            if (!string.IsNullOrWhiteSpace(updateDto.Rut) && updateDto.Rut != proveedor.Rut)
            {
                var existingByRut = await _unitOfWork.Proveedores.GetAsync(
                    p => p.Rut == updateDto.Rut && p.EmpresaId == proveedor.EmpresaId && p.Id != id
                );

                if (existingByRut != null)
                {
                    return BadRequest($"Ya existe otro proveedor con RUT {updateDto.Rut} en esta empresa");
                }
            }

            _mapper.Map(updateDto, proveedor);

            await _unitOfWork.Proveedores.UpdateAsync(proveedor);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Proveedor actualizado: ID {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar proveedor {Id}", id);
            return StatusCode(500, "Error al actualizar proveedor");
        }
    }

    // DELETE: api/proveedores/{id}
    /// <summary>
    /// Elimina un proveedor. Solo AdminGlobal y Administrador.
    /// Nota: Solo puede eliminarse si no tiene precios asociados.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminGlobalOrAdminEmpresa")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            var proveedor = await _unitOfWork.Proveedores.GetByIdAsync(
                id,
                include: query => query.Include(p => p.PreciosProveedor)
            );

            if (proveedor == null)
            {
                return NotFound($"Proveedor con ID {id} no encontrado");
            }

            // Validar acceso
            if (userRole != "AdminGlobal" && proveedor.EmpresaId != empresaId)
            {
                return Forbid("No tienes acceso a este proveedor");
            }

            // Validar que no existan precios asociados
            if (proveedor.PreciosProveedor.Count > 0)
            {
                return BadRequest(
                    "No se puede eliminar un proveedor que tiene precios asociados. " +
                    "Por favor, elimina los precios primero o desactiva el proveedor."
                );
            }

            await _unitOfWork.Proveedores.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Proveedor eliminado: ID {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar proveedor {Id}", id);
            return StatusCode(500, "Error al eliminar proveedor");
        }
    }
}