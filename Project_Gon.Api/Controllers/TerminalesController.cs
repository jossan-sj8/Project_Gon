namespace Project_Gon.Api.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Gon.Core.DTOs.Terminal;
using Project_Gon.Core.Entities;
using Project_Gon.Infrastructure.Repositories;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TerminalesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<TerminalesController> _logger;

    public TerminalesController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TerminalesController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: api/terminales
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TerminalDto>>> GetTerminales([FromQuery] int? sucursalId = null)
    {
        try
        {
            // FIX: Usar 'predicate' en lugar de 'filter'
            var terminales = await _unitOfWork.CajasRegistradoras.GetAllAsync(
                predicate: sucursalId.HasValue ? c => c.SucursalId == sucursalId.Value : null,
                include: q => q.Include(c => c.Sucursal!)
            );

            var terminalesDto = _mapper.Map<IEnumerable<TerminalDto>>(terminales);
            return Ok(terminalesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener terminales");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // GET: api/terminales/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TerminalDto>> GetTerminal(int id)
    {
        try
        {
            var terminal = await _unitOfWork.CajasRegistradoras.GetByIdAsync(
                id,
                include: q => q.Include(c => c.Sucursal!)
            );

            if (terminal == null)
            {
                _logger.LogWarning("Terminal con ID {TerminalId} no encontrado", id);
                return NotFound($"Terminal con ID {id} no encontrado");
            }

            var terminalDto = _mapper.Map<TerminalDto>(terminal);
            return Ok(terminalDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener terminal {TerminalId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // POST: api/terminales
    [HttpPost]
    [Authorize(Roles = "AdminGlobal,AdminEmpresa,AdminSucursal")]
    public async Task<ActionResult<TerminalDto>> CreateTerminal(CreateTerminalDto createDto)
    {
        try
        {
            // Validar que la sucursal existe
            var sucursal = await _unitOfWork.Sucursales.GetByIdAsync(createDto.SucursalId);
            if (sucursal == null)
            {
                _logger.LogWarning("Sucursal con ID {SucursalId} no encontrada", createDto.SucursalId);
                return BadRequest($"Sucursal con ID {createDto.SucursalId} no encontrada");
            }

            // FIX: Validar unicidad de nombre por sucursal (usar 'predicate')
            var nombreExiste = await _unitOfWork.CajasRegistradoras.GetAllAsync(
                predicate: c => c.SucursalId == createDto.SucursalId && c.Nombre == createDto.Nombre
            );

            if (nombreExiste.Any())
            {
                return BadRequest($"Ya existe un terminal con el nombre '{createDto.Nombre}' en esta sucursal");
            }

            // FIX: Validar unicidad de NumeroSerie (si se proporciona)
            if (!string.IsNullOrEmpty(createDto.NumeroSerie))
            {
                var serieExiste = await _unitOfWork.CajasRegistradoras.GetAllAsync(
                    predicate: c => c.NumeroSerie == createDto.NumeroSerie
                );

                if (serieExiste.Any())
                {
                    return BadRequest($"Ya existe un terminal con el número de serie '{createDto.NumeroSerie}'");
                }
            }

            var terminal = _mapper.Map<CajaRegistradora>(createDto);
            terminal.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.CajasRegistradoras.AddAsync(terminal);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Terminal creado: ID {TerminalId}, Nombre {Nombre}, Sucursal {SucursalId}",
                terminal.Id, terminal.Nombre, terminal.SucursalId);

            var terminalCreado = await _unitOfWork.CajasRegistradoras.GetByIdAsync(
                terminal.Id,
                include: q => q.Include(c => c.Sucursal!)
            );

            var terminalDto = _mapper.Map<TerminalDto>(terminalCreado);
            return CreatedAtAction(nameof(GetTerminal), new { id = terminal.Id }, terminalDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear terminal");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // PUT: api/terminales/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "AdminGlobal,AdminEmpresa,AdminSucursal")]
    public async Task<ActionResult<TerminalDto>> UpdateTerminal(int id, UpdateTerminalDto updateDto)
    {
        try
        {
            var terminal = await _unitOfWork.CajasRegistradoras.GetByIdAsync(
                id,
                include: q => q.Include(c => c.Sucursal!)
            );

            if (terminal == null)
            {
                _logger.LogWarning("Terminal con ID {TerminalId} no encontrado", id);
                return NotFound($"Terminal con ID {id} no encontrado");
            }

            // FIX: Validar unicidad de nombre por sucursal (usar 'predicate')
            if (!string.IsNullOrEmpty(updateDto.Nombre))
            {
                var nombreExiste = await _unitOfWork.CajasRegistradoras.GetAllAsync(
                    predicate: c => c.SucursalId == terminal.SucursalId &&
                                    c.Nombre == updateDto.Nombre &&
                                    c.Id != id
                );

                if (nombreExiste.Any())
                {
                    return BadRequest($"Ya existe otro terminal con el nombre '{updateDto.Nombre}' en esta sucursal");
                }

                terminal.Nombre = updateDto.Nombre;
            }

            // FIX: Validar unicidad de NumeroSerie (usar 'predicate')
            if (updateDto.NumeroSerie != null)
            {
                var serieExiste = await _unitOfWork.CajasRegistradoras.GetAllAsync(
                    predicate: c => c.NumeroSerie == updateDto.NumeroSerie && c.Id != id
                );

                if (serieExiste.Any())
                {
                    return BadRequest($"Ya existe otro terminal con el número de serie '{updateDto.NumeroSerie}'");
                }

                terminal.NumeroSerie = updateDto.NumeroSerie;
            }

            if (updateDto.Activo.HasValue)
                terminal.Activo = updateDto.Activo.Value;

            terminal.UpdatedAt = DateTime.UtcNow;

            // FIX: Mantener UpdateAsync() (tu Repository lo tiene)
            await _unitOfWork.CajasRegistradoras.UpdateAsync(terminal);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Terminal actualizado: ID {TerminalId}", id);

            var terminalDto = _mapper.Map<TerminalDto>(terminal);
            return Ok(terminalDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar terminal {TerminalId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // DELETE: api/terminales/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "AdminGlobal,AdminEmpresa")]
    public async Task<IActionResult> DeleteTerminal(int id)
    {
        try
        {
            var terminal = await _unitOfWork.CajasRegistradoras.GetByIdAsync(id);
            if (terminal == null)
            {
                _logger.LogWarning("Terminal con ID {TerminalId} no encontrado", id);
                return NotFound($"Terminal con ID {id} no encontrado");
            }

            // FIX: Validar que no tenga arqueos de caja asociados (usar 'predicate')
            var tieneArqueos = await _unitOfWork.ArqueosCaja.GetAllAsync(
                predicate: a => a.CajaRegistradoraId == id
            );

            if (tieneArqueos.Any())
            {
                return BadRequest("No se puede eliminar el terminal porque tiene arqueos de caja asociados");
            }

            // FIX: Mantener DeleteAsync() (tu Repository lo tiene)
            await _unitOfWork.CajasRegistradoras.DeleteAsync(terminal);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Terminal eliminado: ID {TerminalId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar terminal {TerminalId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // GET: api/terminales/sucursal/{sucursalId}
    [HttpGet("sucursal/{sucursalId}")]
    public async Task<ActionResult<IEnumerable<TerminalDto>>> GetTerminalesBySucursal(int sucursalId)
    {
        try
        {
            // FIX: Usar 'predicate' en lugar de 'filter'
            var terminales = await _unitOfWork.CajasRegistradoras.GetAllAsync(
                predicate: c => c.SucursalId == sucursalId,
                include: q => q.Include(c => c.Sucursal!)
            );

            var terminalesDto = _mapper.Map<IEnumerable<TerminalDto>>(terminales);
            return Ok(terminalesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener terminales de la sucursal {SucursalId}", sucursalId);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}