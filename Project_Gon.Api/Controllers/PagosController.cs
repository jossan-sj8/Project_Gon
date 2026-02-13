namespace Project_Gon.Api.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Gon.Core.DTOs.Transaccion;
using Project_Gon.Core.Entities;
using Project_Gon.Infrastructure.Repositories;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PagosController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<PagosController> _logger;

    public PagosController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PagosController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: api/pagos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PagoDto>>> GetPagos([FromQuery] int? ventaId = null)
    {
        try
        {
            var query = await _unitOfWork.Pagos.GetAllAsync(
                include: q => q.Include(p => p.MetodoPago)
            );

            if (ventaId.HasValue)
            {
                query = query.Where(p => p.VentaId == ventaId.Value).ToList();
            }

            var pagosDto = _mapper.Map<IEnumerable<PagoDto>>(query);
            return Ok(pagosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener pagos");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // GET: api/pagos/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<PagoDto>> GetPago(int id)
    {
        try
        {
            var pago = await _unitOfWork.Pagos.GetByIdAsync(
                id,
                include: q => q.Include(p => p.MetodoPago)
            );

            if (pago == null)
            {
                _logger.LogWarning("Pago con ID {PagoId} no encontrado", id);
                return NotFound($"Pago con ID {id} no encontrado");
            }

            var pagoDto = _mapper.Map<PagoDto>(pago);
            return Ok(pagoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener pago {PagoId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // POST: api/pagos
    [HttpPost]
    public async Task<ActionResult<PagoDto>> CreatePago(CreatePagoDto createPagoDto)
    {
        try
        {
            // Validar que la venta existe
            var venta = await _unitOfWork.Ventas.GetByIdAsync(createPagoDto.VentaId);
            if (venta == null)
            {
                _logger.LogWarning("Venta con ID {VentaId} no encontrada", createPagoDto.VentaId);
                return NotFound($"Venta con ID {createPagoDto.VentaId} no encontrada");
            }

            // Validar que el método de pago existe y está activo
            var metodoPago = await _unitOfWork.MetodosPago.GetByIdAsync(createPagoDto.MetodoPagoId);
            if (metodoPago == null || !metodoPago.Activo)
            {
                _logger.LogWarning("Método de pago con ID {MetodoPagoId} no encontrado o inactivo", createPagoDto.MetodoPagoId);
                return BadRequest("Método de pago no válido");
            }

            var pago = _mapper.Map<Pago>(createPagoDto);
            pago.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Pagos.AddAsync(pago);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Pago creado exitosamente: ID {PagoId}, Venta {VentaId}, Monto ${Monto}",
                pago.Id, pago.VentaId, pago.Monto);

            var pagoCreado = await _unitOfWork.Pagos.GetByIdAsync(
                pago.Id,
                include: q => q.Include(p => p.MetodoPago)
            );

            var pagoDto = _mapper.Map<PagoDto>(pagoCreado);
            return CreatedAtAction(nameof(GetPago), new { id = pago.Id }, pagoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear pago");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // DELETE: api/pagos/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "AdminGlobal,AdminEmpresa")]
    public async Task<IActionResult> DeletePago(int id)
    {
        try
        {
            var pago = await _unitOfWork.Pagos.GetByIdAsync(id);
            if (pago == null)
            {
                _logger.LogWarning("Pago con ID {PagoId} no encontrado para eliminar", id);
                return NotFound($"Pago con ID {id} no encontrado");
            }

            await _unitOfWork.Pagos.DeleteAsync(pago);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Pago eliminado: ID {PagoId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar pago {PagoId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}