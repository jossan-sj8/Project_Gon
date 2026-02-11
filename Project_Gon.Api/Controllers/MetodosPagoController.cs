using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_Gon.Api.Extensions;
using Project_Gon.Core.DTOs.MetodoPago;
using Project_Gon.Core.Entities;
using Project_Gon.Infrastructure.Repositories;
using System.Linq.Expressions;

namespace Project_Gon.Api.Controllers;

/// <summary>
/// Controlador para gestionar métodos de pago.
/// Endpoints CRUD con autorización basada en roles (JWT).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MetodosPagoController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<MetodosPagoController> _logger;

    public MetodosPagoController(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<MetodosPagoController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: api/metodospago
    /// <summary>
    /// Obtiene todos los métodos de pago.
    /// Disponible para todos los roles autenticados.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MetodoPagoDto>>> GetAll([FromQuery] bool? activo = null)
    {
        try
        {
            // ✅ CORREGIDO: Usar sobrecarga sin predicate cuando es null
            IEnumerable<MetodoPago> metodosPago;

            if (activo.HasValue)
            {
                metodosPago = await _unitOfWork.MetodosPago.GetAllAsync(m => m.Activo == activo.Value);
            }
            else
            {
                metodosPago = await _unitOfWork.MetodosPago.GetAllAsync();
            }

            var metodosPagoDto = _mapper.Map<IEnumerable<MetodoPagoDto>>(metodosPago);

            _logger.LogInformation(
                "Retrieved {Count} métodos de pago for user {UserId}",
                metodosPagoDto.Count(),
                User.GetUserId()
            );

            return Ok(metodosPagoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener métodos de pago");
            return StatusCode(500, "Error al obtener métodos de pago");
        }
    }

    // GET: api/metodospago/{id}
    /// <summary>
    /// Obtiene un método de pago específico por su ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<MetodoPagoDto>> GetById(int id)
    {
        try
        {
            var metodoPago = await _unitOfWork.MetodosPago.GetByIdAsync(id);

            if (metodoPago == null)
            {
                _logger.LogWarning("Método de pago con ID {Id} no encontrado", id);
                return NotFound($"Método de pago con ID {id} no encontrado");
            }

            var metodoPagoDto = _mapper.Map<MetodoPagoDto>(metodoPago);
            return Ok(metodoPagoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener método de pago {Id}", id);
            return StatusCode(500, "Error al obtener método de pago");
        }
    }

    // POST: api/metodospago
    /// <summary>
    /// Crea un nuevo método de pago. Solo AdminGlobal.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "AdminGlobal")]
    public async Task<ActionResult<MetodoPagoDto>> Create([FromBody] CreateMetodoPagoDto createDto)
    {
        try
        {
            // Validar unicidad del nombre
            var existing = await _unitOfWork.MetodosPago.GetAsync(m => m.Nombre == createDto.Nombre);
            if (existing != null)
            {
                return BadRequest($"Ya existe un método de pago con el nombre '{createDto.Nombre}'");
            }

            var metodoPago = _mapper.Map<MetodoPago>(createDto);

            await _unitOfWork.MetodosPago.AddAsync(metodoPago);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Método de pago creado: ID {Id}, Nombre {Nombre}",
                metodoPago.Id, metodoPago.Nombre
            );

            var metodoPagoDto = _mapper.Map<MetodoPagoDto>(metodoPago);

            return CreatedAtAction(nameof(GetById), new { id = metodoPago.Id }, metodoPagoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear método de pago");
            return StatusCode(500, "Error al crear método de pago");
        }
    }

    // PUT: api/metodospago/{id}
    /// <summary>
    /// Actualiza un método de pago existente. Solo AdminGlobal.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "AdminGlobal")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMetodoPagoDto updateDto)
    {
        try
        {
            var metodoPago = await _unitOfWork.MetodosPago.GetByIdAsync(id);

            if (metodoPago == null)
            {
                return NotFound($"Método de pago con ID {id} no encontrado");
            }

            // Validar unicidad del nombre si se está cambiando
            if (!string.IsNullOrWhiteSpace(updateDto.Nombre) && updateDto.Nombre != metodoPago.Nombre)
            {
                var existing = await _unitOfWork.MetodosPago.GetAsync(
                    m => m.Nombre == updateDto.Nombre && m.Id != id
                );

                if (existing != null)
                {
                    return BadRequest($"Ya existe otro método de pago con el nombre '{updateDto.Nombre}'");
                }
            }

            _mapper.Map(updateDto, metodoPago);

            await _unitOfWork.MetodosPago.UpdateAsync(metodoPago);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Método de pago actualizado: ID {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar método de pago {Id}", id);
            return StatusCode(500, "Error al actualizar método de pago");
        }
    }

    // DELETE: api/metodospago/{id}
    /// <summary>
    /// Elimina un método de pago. Solo AdminGlobal.
    /// Nota: Solo puede eliminarse si no tiene pagos asociados.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "AdminGlobal")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var metodoPago = await _unitOfWork.MetodosPago.GetByIdAsync(id);

            if (metodoPago == null)
            {
                return NotFound($"Método de pago con ID {id} no encontrado");
            }

            // Validar que no existan pagos asociados (consulta manual)
            var hasPagos = await _unitOfWork.Pagos.GetAllAsync(p => p.MetodoPagoId == id);
            if (hasPagos.Any())
            {
                return BadRequest(
                    "No se puede eliminar un método de pago que tiene pagos asociados. " +
                    "Por favor, desactiva el método de pago en su lugar."
                );
            }

            await _unitOfWork.MetodosPago.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Método de pago eliminado: ID {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar método de pago {Id}", id);
            return StatusCode(500, "Error al eliminar método de pago");
        }
    }
}