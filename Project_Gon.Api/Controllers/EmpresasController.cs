using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Project_Gon.Core.DTOs.Empresa;
using Project_Gon.Core.Entities;
using Project_Gon.Infrastructure.Repositories;

namespace Project_Gon.Api.Controllers;

/// <summary>
/// Controlador para gestionar empresas.
/// Endpoints CRUD para crear, leer, actualizar y eliminar empresas.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EmpresasController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EmpresasController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Obtiene todas las empresas.
    /// GET: api/empresas
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EmpresaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var empresas = await _unitOfWork.Empresas.GetAllAsync();
        var empresasDto = _mapper.Map<IEnumerable<EmpresaDto>>(empresas);
        return Ok(empresasDto);
    }

    /// <summary>
    /// Obtiene una empresa por su ID.
    /// GET: api/empresas/1
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EmpresaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
            return BadRequest("El ID debe ser mayor a 0");

        var empresa = await _unitOfWork.Empresas.GetByIdAsync(id);

        if (empresa == null)
            return NotFound($"Empresa con ID {id} no encontrada");

        var empresaDto = _mapper.Map<EmpresaDto>(empresa);
        return Ok(empresaDto);
    }

    /// <summary>
    /// Crea una nueva empresa.
    /// POST: api/empresas
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EmpresaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateEmpresaDto createDto)
    {
        // Validar modelo
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Validar nombre no vacío
        if (string.IsNullOrWhiteSpace(createDto.Nombre))
            return BadRequest("El nombre de la empresa es requerido");

        try
        {
            // Mapear DTO → Entity
            var empresa = _mapper.Map<Empresa>(createDto);

            // Guardar en BD
            await _unitOfWork.Empresas.AddAsync(empresa);
            await _unitOfWork.SaveChangesAsync();

            // Mapear Entity → DTO
            var resultado = _mapper.Map<EmpresaDto>(empresa);

            // Retornar con status 201 Created
            return CreatedAtAction(nameof(GetById), new { id = resultado.Id }, resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { mensaje = "Error al crear la empresa", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza una empresa existente.
    /// PUT: api/empresas/1
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(EmpresaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmpresaDto updateDto)
    {
        // Validar ID
        if (id <= 0)
            return BadRequest("El ID debe ser mayor a 0");

        // Validar modelo
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // Obtener empresa existente
            var empresa = await _unitOfWork.Empresas.GetByIdAsync(id);

            if (empresa == null)
                return NotFound($"Empresa con ID {id} no encontrada");

            // Mapear DTO → Entity (actualización parcial)
            _mapper.Map(updateDto, empresa);

            // Actualizar en BD
            await _unitOfWork.Empresas.UpdateAsync(empresa);
            await _unitOfWork.SaveChangesAsync();

            // Mapear Entity → DTO
            var resultado = _mapper.Map<EmpresaDto>(empresa);

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { mensaje = "Error al actualizar la empresa", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Elimina una empresa por su ID.
    /// DELETE: api/empresas/1
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        // Validar ID
        if (id <= 0)
            return BadRequest("El ID debe ser mayor a 0");

        try
        {
            // Eliminar de BD
            var resultado = await _unitOfWork.Empresas.DeleteAsync(id);

            if (!resultado)
                return NotFound($"Empresa con ID {id} no encontrada");

            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { mensaje = "Error al eliminar la empresa", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene todas las empresas activas.
    /// GET: api/empresas/activas
    /// </summary>
    [HttpGet("activas")]
    [ProducesResponseType(typeof(IEnumerable<EmpresaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActivas()
    {
        var empresas = await _unitOfWork.Empresas.GetAllAsync();
        var empresasActivas = empresas.Where(e => e.Activo);
        var empresasDto = _mapper.Map<IEnumerable<EmpresaDto>>(empresasActivas);
        return Ok(empresasDto);
    }
}