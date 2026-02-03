using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Gon.Core.DTOs.Cliente;
using Project_Gon.Core.Entities;
using Project_Gon.Infrastructure.Repositories;
using Project_Gon.Api.Extensions;
using System.Linq.Expressions;

namespace Project_Gon.Api.Controllers;

/// <summary>
/// Controlador para gestionar clientes del negocio.
/// Endpoints CRUD con autorización basada en roles (JWT).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ClientesController> _logger;

    public ClientesController(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ClientesController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: api/clientes
    /// <summary>
    /// Obtiene todos los clientes según el rol del usuario autenticado.
    /// - AdminGlobal: todos los clientes
    /// - AdminEmpresa/Vendedor: solo clientes de su empresa
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteDto>>> GetAll()
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            Expression<Func<Cliente, bool>>? predicate = null;

            if (userRole == "AdminGlobal")
            {
                predicate = null;
            }
            else
            {
                predicate = c => c.EmpresaId == empresaId;
            }

            var clientes = await _unitOfWork.Clientes.GetAllAsync(
                predicate: predicate,
                include: query => query.Include(c => c.Empresa)
            );

            var clientesDto = _mapper.Map<IEnumerable<ClienteDto>>(clientes);

            _logger.LogInformation(
                "Retrieved {Count} clientes for user {UserId} with role {Role}",
                clientesDto.Count(),
                User.GetUserId(),
                userRole
            );

            return Ok(clientesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener clientes");
            return StatusCode(500, "Error al obtener clientes");
        }
    }

    // GET: api/clientes/{id}
    /// <summary>
    /// Obtiene un cliente por su ID con validación de acceso.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ClienteDto>> GetById(int id)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            var cliente = await _unitOfWork.Clientes.GetByIdAsync(
                id,
                include: query => query.Include(c => c.Empresa)
            );

            if (cliente == null)
            {
                _logger.LogWarning("Cliente con ID {Id} no encontrado", id);
                return NotFound($"Cliente con ID {id} no encontrado");
            }

            if (userRole != "AdminGlobal" && cliente.EmpresaId != empresaId)
            {
                return Forbid("No tienes acceso a este cliente");
            }

            var clienteDto = _mapper.Map<ClienteDto>(cliente);
            return Ok(clienteDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cliente {Id}", id);
            return StatusCode(500, "Error al obtener cliente");
        }
    }

    // POST: api/clientes
    /// <summary>
    /// Crea un nuevo cliente. Solo AdminGlobal y AdminEmpresa.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "AdminGlobal,AdminEmpresa")]
    public async Task<ActionResult<ClienteDto>> Create([FromBody] CreateClienteDto createDto)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            if (userRole != "AdminGlobal" && createDto.EmpresaId != empresaId)
            {
                return Forbid("No puedes crear clientes en otras empresas");
            }

            var empresa = await _unitOfWork.Empresas.GetByIdAsync(createDto.EmpresaId);
            if (empresa == null)
            {
                return BadRequest($"La empresa con ID {createDto.EmpresaId} no existe");
            }

            var cliente = _mapper.Map<Cliente>(createDto);

            await _unitOfWork.Clientes.AddAsync(cliente);
            await _unitOfWork.SaveChangesAsync();

            cliente = await _unitOfWork.Clientes.GetByIdAsync(
                cliente.Id,
                include: query => query.Include(c => c.Empresa)
            ) ?? throw new InvalidOperationException("Cliente no encontrado después de crearlo");

            var clienteDto = _mapper.Map<ClienteDto>(cliente);

            _logger.LogInformation("Cliente creado: {Nombre} - ID {Id}",
                cliente.Nombre, cliente.Id);

            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, clienteDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear cliente");
            return StatusCode(500, "Error al crear cliente");
        }
    }

    // PUT: api/clientes/{id}
    /// <summary>
    /// Actualiza un cliente existente. Solo AdminGlobal y AdminEmpresa.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "AdminGlobal,AdminEmpresa")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClienteDto updateDto)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            var cliente = await _unitOfWork.Clientes.GetByIdAsync(
                id,
                include: query => query.Include(c => c.Empresa)
            );

            if (cliente == null)
            {
                return NotFound($"Cliente con ID {id} no encontrado");
            }

            if (userRole != "AdminGlobal" && cliente.EmpresaId != empresaId)
            {
                return Forbid("No tienes acceso a este cliente");
            }

            _mapper.Map(updateDto, cliente);

            await _unitOfWork.Clientes.UpdateAsync(cliente);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Cliente actualizado: ID {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar cliente {Id}", id);
            return StatusCode(500, "Error al actualizar cliente");
        }
    }

    // DELETE: api/clientes/{id}
    /// <summary>
    /// Elimina un cliente. Solo AdminGlobal y AdminEmpresa.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "AdminGlobal,AdminEmpresa")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();

            var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);

            if (cliente == null)
            {
                return NotFound($"Cliente con ID {id} no encontrado");
            }

            if (userRole != "AdminGlobal" && cliente.EmpresaId != empresaId)
            {
                return Forbid("No tienes acceso a este cliente");
            }

            await _unitOfWork.Clientes.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Cliente eliminado: ID {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar cliente {Id}", id);
            return StatusCode(500, "Error al eliminar cliente");
        }
    }
}