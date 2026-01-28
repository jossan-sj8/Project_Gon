using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Project_Gon.Core.DTOs.Sucursal;
using Project_Gon.Core.Entities;
using Project_Gon.Infrastructure.Repositories;

namespace Project_Gon.Api.Controllers;

/// <summary>
/// Controlador para gestionar sucursales
/// Endpoints CRUD para crear, leer, actualizar y eliminar sucursales
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SucursalesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SucursalesController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Obtiene todas las sucursales
    /// GET: api/sucursales
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SucursalDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var sucursales = await _unitOfWork.Sucursales.GetAllAsync();

        // Cargar empresas manualmente
        foreach (var sucursal in sucursales)
        {
            var empresa = await _unitOfWork.Empresas.GetByIdAsync(sucursal.EmpresaId);
            if (empresa != null)
            {
                sucursal.Empresa = empresa;
            }
        }

        var sucursalesDto = _mapper.Map<IEnumerable<SucursalDto>>(sucursales);
        return Ok(sucursalesDto);
    }

    /// <summary>
    /// Obtiene una sucursal por su ID
    /// GET: api/sucursales/1
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SucursalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
            return BadRequest("El ID debe ser mayor a 0");

        var sucursal = await _unitOfWork.Sucursales.GetByIdAsync(id);

        if (sucursal == null)
            return NotFound($"Sucursal con ID {id} no encontrada");

        // Cargar empresa manualmente
        var empresa = await _unitOfWork.Empresas.GetByIdAsync(sucursal.EmpresaId);
        if (empresa != null)
        {
            sucursal.Empresa = empresa;
        }

        var sucursalDto = _mapper.Map<SucursalDto>(sucursal);
        return Ok(sucursalDto);
    }

    /// <summary>
    /// Obtiene todas las sucursales de una empresa específica
    /// GET: api/sucursales/empresa/1
    /// </summary>
    [HttpGet("empresa/{empresaId}")]
    [ProducesResponseType(typeof(IEnumerable<SucursalDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByEmpresa(int empresaId)
    {
        if (empresaId <= 0)
            return BadRequest("El ID de empresa debe ser mayor a 0");

        // Verificar que la empresa existe
        var empresa = await _unitOfWork.Empresas.GetByIdAsync(empresaId);
        if (empresa == null)
            return NotFound($"Empresa con ID {empresaId} no encontrada");

        var sucursales = await _unitOfWork.Sucursales.GetAllAsync();
        var sucursalesEmpresa = sucursales.Where(s => s.EmpresaId == empresaId).ToList();

        // Cargar empresas manualmente
        foreach (var sucursal in sucursalesEmpresa)
        {
            sucursal.Empresa = empresa;
        }

        var sucursalesDto = _mapper.Map<IEnumerable<SucursalDto>>(sucursalesEmpresa);
        return Ok(sucursalesDto);
    }

    /// <summary>
    /// Crea una nueva sucursal
    /// POST: api/sucursales
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SucursalDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateSucursalDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(createDto.Nombre))
            return BadRequest("El nombre de la sucursal es requerido");

        try
        {
            // Verificar que la empresa existe
            var empresa = await _unitOfWork.Empresas.GetByIdAsync(createDto.EmpresaId);
            if (empresa == null)
                return BadRequest($"Empresa con ID {createDto.EmpresaId} no encontrada");

            // Mapear DTO → Entity
            var sucursal = _mapper.Map<Sucursal>(createDto);

            // Guardar en BD
            await _unitOfWork.Sucursales.AddAsync(sucursal);
            await _unitOfWork.SaveChangesAsync();

            // Cargar empresa para el response
            sucursal.Empresa = empresa;

            // Mapear Entity → DTO
            var resultado = _mapper.Map<SucursalDto>(sucursal);

            return CreatedAtAction(nameof(GetById), new { id = resultado.Id }, resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { mensaje = "Error al crear la sucursal", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza una sucursal existente
    /// PUT: api/sucursales/1
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SucursalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSucursalDto updateDto)
    {
        if (id <= 0)
            return BadRequest("El ID debe ser mayor a 0");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // Obtener sucursal existente
            var sucursal = await _unitOfWork.Sucursales.GetByIdAsync(id);

            if (sucursal == null)
                return NotFound($"Sucursal con ID {id} no encontrada");

            // Mapear DTO → Entity (actualización parcial)
            _mapper.Map(updateDto, sucursal);

            // Actualizar en BD
            await _unitOfWork.Sucursales.UpdateAsync(sucursal);
            await _unitOfWork.SaveChangesAsync();

            // Cargar empresa para el response
            var empresa = await _unitOfWork.Empresas.GetByIdAsync(sucursal.EmpresaId);
            if (empresa != null)
            {
                sucursal.Empresa = empresa;
            }

            // Mapear Entity → DTO
            var resultado = _mapper.Map<SucursalDto>(sucursal);

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { mensaje = "Error al actualizar la sucursal", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Elimina una sucursal por su ID
    /// DELETE: api/sucursales/1
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return BadRequest("El ID debe ser mayor a 0");

        try
        {
            // Verificar si hay usuarios asignados a esta sucursal
            var usuarios = await _unitOfWork.Usuarios.GetAllAsync();
            var usuariosEnSucursal = usuarios.Any(u => u.SucursalId == id);

            if (usuariosEnSucursal)
                return BadRequest("No se puede eliminar la sucursal porque tiene usuarios asignados");

            // Eliminar de BD
            var resultado = await _unitOfWork.Sucursales.DeleteAsync(id);

            if (!resultado)
                return NotFound($"Sucursal con ID {id} no encontrada");

            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { mensaje = "Error al eliminar la sucursal", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene todas las sucursales activas
    /// GET: api/sucursales/activas
    /// </summary>
    [HttpGet("activas")]
    [ProducesResponseType(typeof(IEnumerable<SucursalDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActivas()
    {
        var sucursales = await _unitOfWork.Sucursales.GetAllAsync();
        var sucursalesActivas = sucursales.Where(s => s.Activo).ToList();

        // Cargar empresas manualmente
        foreach (var sucursal in sucursalesActivas)
        {
            var empresa = await _unitOfWork.Empresas.GetByIdAsync(sucursal.EmpresaId);
            if (empresa != null)
            {
                sucursal.Empresa = empresa;
            }
        }

        var sucursalesDto = _mapper.Map<IEnumerable<SucursalDto>>(sucursalesActivas);
        return Ok(sucursalesDto);
    }
}