using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Gon.Core.DTOs.Usuario;
using Project_Gon.Core.Entities;
using Project_Gon.Core.Enums;
using Project_Gon.Infrastructure.Repositories;
using Project_Gon.Infrastructure.Services;
using Project_Gon.Api.Extensions;  
using System.Linq.Expressions;     

namespace Project_Gon.Api.Controllers;

/// <summary>
/// Controlador para gestionar usuarios del sistema.
/// Endpoints CRUD con autorización basada en roles (JWT).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Requiere autenticación JWT
public class UsuariosController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPasswordHashService _passwordHashService;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IPasswordHashService passwordHashService,
        ILogger<UsuariosController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordHashService = passwordHashService;
        _logger = logger;
    }

    // GET: api/usuarios
    /// <summary>
    /// Obtiene usuarios según el rol del usuario autenticado.
    /// - AdminGlobal: todos los usuarios
    /// - AdminEmpresa sin sucursal: usuarios de su empresa
    /// - AdminEmpresa con sucursal: usuarios de su sucursal
    /// - Vendedor: usuarios de su sucursal
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetAll()
    {
        try
        {
            // Extraer información del usuario actual desde el JWT
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();
            var sucursalId = User.GetSucursalId();

            Expression<Func<Usuario, bool>>? predicate = null;

            // Determinar filtro según rol
            if (userRole == "AdminGlobal")
            {
                // AdminGlobal: sin filtro (ve todos)
                predicate = null;
            }
            else if (userRole == "AdminEmpresa")
            {
                if (sucursalId.HasValue)
                {
                    // AdminEmpresa con sucursal: solo su sucursal
                    predicate = u => u.SucursalId == sucursalId.Value;
                }
                else
                {
                    // AdminEmpresa sin sucursal: toda su empresa
                    predicate = u => u.EmpresaId == empresaId;
                }
            }
            else if (userRole == "Vendedor")
            {
                // Vendedor: solo su sucursal
                predicate = u => u.SucursalId == sucursalId;
            }
            else
            {
                return Unauthorized("Rol no reconocido");
            }

            // Cargar usuarios con relaciones
            var usuarios = await _unitOfWork.Usuarios.GetAllAsync(
                predicate: predicate,
                include: query => query
                    .Include(u => u.Empresa)
                    .Include(u => u.Sucursal)!
            );

            var usuariosDto = _mapper.Map<IEnumerable<UsuarioDto>>(usuarios);

            _logger.LogInformation(
                "Retrieved {Count} usuarios for user {UserId} with role {Role}",
                usuariosDto.Count(),
                User.GetUserId(),
                userRole
            );

            return Ok(usuariosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuarios");
            return StatusCode(500, "Error al obtener usuarios");
        }
    }

    // GET: api/usuarios/{id}
    /// <summary>
    /// Obtiene un usuario por su ID con validación de acceso según rol.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<UsuarioDto>> GetById(int id)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();
            var sucursalId = User.GetSucursalId();

            // Cargar usuario
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(
                id,
                include: query => query
                    .Include(u => u.Empresa)
                    .Include(u => u.Sucursal)!
            );

            if (usuario == null)
            {
                _logger.LogWarning("Usuario con ID {Id} no encontrado", id);
                return NotFound($"Usuario con ID {id} no encontrado");
            }

            // Validar acceso según rol
            if (userRole == "AdminGlobal")
            {
                // AdminGlobal puede ver a todos
            }
            else if (userRole == "AdminEmpresa")
            {
                if (sucursalId.HasValue)
                {
                    // AdminEmpresa con sucursal: solo de su sucursal
                    if (usuario.SucursalId != sucursalId.Value)
                        return Forbid("No tienes acceso a este usuario");
                }
                else
                {
                    // AdminEmpresa sin sucursal: solo de su empresa
                    if (usuario.EmpresaId != empresaId)
                        return Forbid("No tienes acceso a este usuario");
                }
            }
            else if (userRole == "Vendedor")
            {
                // Vendedor: solo de su sucursal
                if (usuario.SucursalId != sucursalId)
                    return Forbid("No tienes acceso a este usuario");
            }

            var usuarioDto = _mapper.Map<UsuarioDto>(usuario);
            return Ok(usuarioDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario {Id}", id);
            return StatusCode(500, "Error al obtener usuario");
        }
    }

    // GET: api/usuarios/empresa/{empresaId}
    /// <summary>
    /// Obtiene todos los usuarios de una empresa específica (solo AdminGlobal).
    /// </summary>
    [HttpGet("empresa/{empresaId}")]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetByEmpresa(int empresaId)
    {
        try
        {
            var userRole = User.GetUserRole();
            var currentEmpresaId = User.GetEmpresaId();

            // Solo AdminGlobal puede ver usuarios de otras empresas
            if (userRole != "AdminGlobal" && currentEmpresaId != empresaId)
            {
                return Forbid("No tienes acceso a usuarios de otras empresas");
            }

            var usuarios = await _unitOfWork.Usuarios.GetAllAsync(
                predicate: u => u.EmpresaId == empresaId,
                include: query => query
                    .Include(u => u.Empresa)
                    .Include(u => u.Sucursal)!
            );

            var usuariosDto = _mapper.Map<IEnumerable<UsuarioDto>>(usuarios);

            _logger.LogInformation("Retrieved {Count} usuarios for empresa {EmpresaId}",
                usuariosDto.Count(), empresaId);
            return Ok(usuariosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuarios de empresa {EmpresaId}", empresaId);
            return StatusCode(500, "Error al obtener usuarios");
        }
    }

    // GET: api/usuarios/sucursal/{sucursalId}
    /// <summary>
    /// Obtiene todos los usuarios de una sucursal específica.
    /// </summary>
    [HttpGet("sucursal/{sucursalId}")]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetBySucursal(int sucursalId)
    {
        try
        {
            var userRole = User.GetUserRole();
            var currentSucursalId = User.GetSucursalId();

            // Vendedor solo puede ver su propia sucursal
            if (userRole == "Vendedor" && currentSucursalId != sucursalId)
            {
                return Forbid("No tienes acceso a usuarios de otras sucursales");
            }

            var usuarios = await _unitOfWork.Usuarios.GetAllAsync(
                predicate: u => u.SucursalId == sucursalId,
                include: query => query
                    .Include(u => u.Empresa)
                    .Include(u => u.Sucursal)!
            );

            var usuariosDto = _mapper.Map<IEnumerable<UsuarioDto>>(usuarios);

            _logger.LogInformation("Retrieved {Count} usuarios for sucursal {SucursalId}",
                usuariosDto.Count(), sucursalId);
            return Ok(usuariosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuarios de sucursal {SucursalId}", sucursalId);
            return StatusCode(500, "Error al obtener usuarios");
        }
    }

    // GET: api/usuarios/rol/{rol}
    /// <summary>
    /// Obtiene todos los usuarios con un rol específico (solo AdminGlobal).
    /// </summary>
    [HttpGet("rol/{rol}")]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetByRol(RolUsuario rol)
    {
        try
        {
            var userRole = User.GetUserRole();

            // Solo AdminGlobal puede filtrar por rol
            if (userRole != "AdminGlobal")
            {
                return Forbid("No tienes permiso para filtrar usuarios por rol");
            }

            var usuarios = await _unitOfWork.Usuarios.GetAllAsync(
                predicate: u => u.Rol == rol,
                include: query => query
                    .Include(u => u.Empresa)
                    .Include(u => u.Sucursal)!
            );

            var usuariosDto = _mapper.Map<IEnumerable<UsuarioDto>>(usuarios);

            _logger.LogInformation("Retrieved {Count} usuarios with rol {Rol}",
                usuariosDto.Count(), rol);
            return Ok(usuariosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuarios por rol {Rol}", rol);
            return StatusCode(500, "Error al obtener usuarios");
        }
    }

    // GET: api/usuarios/activos
    /// <summary>
    /// Obtiene usuarios activos o inactivos según el rol del usuario autenticado.
    /// </summary>
    [HttpGet("activos")]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetActivos([FromQuery] bool activos = true)
    {
        try
        {
            var userRole = User.GetUserRole();
            var empresaId = User.GetEmpresaId();
            var sucursalId = User.GetSucursalId();

            Expression<Func<Usuario, bool>> predicate;

            // Combinar filtro de activos con filtro de rol
            if (userRole == "AdminGlobal")
            {
                predicate = u => u.Activo == activos;
            }
            else if (userRole == "AdminEmpresa")
            {
                if (sucursalId.HasValue)
                {
                    predicate = u => u.Activo == activos && u.SucursalId == sucursalId.Value;
                }
                else
                {
                    predicate = u => u.Activo == activos && u.EmpresaId == empresaId;
                }
            }
            else if (userRole == "Vendedor")
            {
                predicate = u => u.Activo == activos && u.SucursalId == sucursalId;
            }
            else
            {
                return Unauthorized("Rol no reconocido");
            }

            var usuarios = await _unitOfWork.Usuarios.GetAllAsync(
                predicate: predicate,
                include: query => query
                    .Include(u => u.Empresa)
                    .Include(u => u.Sucursal)!
            );

            var usuariosDto = _mapper.Map<IEnumerable<UsuarioDto>>(usuarios);

            _logger.LogInformation("Retrieved {Count} usuarios {Status}",
                usuariosDto.Count(), activos ? "activos" : "inactivos");
            return Ok(usuariosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuarios activos");
            return StatusCode(500, "Error al obtener usuarios");
        }
    }

    // POST: api/usuarios
    /// <summary>
    /// Crea un nuevo usuario. Solo accesible por AdminGlobal y AdminEmpresa.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "AdminGlobal,AdminEmpresa")]
    public async Task<ActionResult<UsuarioDto>> Create([FromBody] CreateUsuarioDto createDto)
    {
        try
        {
            var empresa = await _unitOfWork.Empresas.GetByIdAsync(createDto.EmpresaId);
            if (empresa == null)
            {
                return BadRequest($"La empresa con ID {createDto.EmpresaId} no existe");
            }

            if (createDto.SucursalId.HasValue)
            {
                var sucursal = await _unitOfWork.Sucursales.GetByIdAsync(createDto.SucursalId.Value);
                if (sucursal == null)
                {
                    return BadRequest($"La sucursal con ID {createDto.SucursalId} no existe");
                }
            }

            var rutExiste = await _unitOfWork.Usuarios.ExistsAsync(
                u => u.Rut == createDto.Rut && u.EmpresaId == createDto.EmpresaId);

            if (rutExiste)
            {
                return BadRequest($"Ya existe un usuario con RUT {createDto.Rut} en esta empresa");
            }

            if (!string.IsNullOrWhiteSpace(createDto.Email))
            {
                var emailExiste = await _unitOfWork.Usuarios.ExistsAsync(
                    u => u.Email == createDto.Email && u.EmpresaId == createDto.EmpresaId);

                if (emailExiste)
                {
                    return BadRequest($"Ya existe un usuario con email {createDto.Email} en esta empresa");
                }
            }

            var usuario = _mapper.Map<Usuario>(createDto);
            usuario.PasswordHash = _passwordHashService.HashPassword(createDto.Password);

            await _unitOfWork.Usuarios.AddAsync(usuario);
            await _unitOfWork.SaveChangesAsync();

            usuario = await _unitOfWork.Usuarios.GetByIdAsync(
                usuario.Id,
                include: query => query
                    .Include(u => u.Empresa)
                    .Include(u => u.Sucursal)!
            ) ?? throw new InvalidOperationException("Usuario no encontrado después de crearlo");

            var usuarioDto = _mapper.Map<UsuarioDto>(usuario);

            _logger.LogInformation("Usuario creado: {Rut} - {Nombre} {Apellido}",
                usuario.Rut, usuario.Nombre, usuario.Apellido);

            return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuarioDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario");
            return StatusCode(500, "Error al crear usuario");
        }
    }

    // PUT: api/usuarios/{id}
    /// <summary>
    /// Actualiza un usuario existente. Solo accesible por AdminGlobal y AdminEmpresa.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "AdminGlobal,AdminEmpresa")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUsuarioDto updateDto)
    {
        try
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(
                id,
                include: query => query
                    .Include(u => u.Empresa)
                    .Include(u => u.Sucursal)!
            );

            if (usuario == null)
            {
                return NotFound($"Usuario con ID {id} no encontrado");
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Email) && updateDto.Email != usuario.Email)
            {
                var emailExiste = await _unitOfWork.Usuarios.ExistsAsync(
                    u => u.Email == updateDto.Email && u.EmpresaId == usuario.EmpresaId && u.Id != id);

                if (emailExiste)
                {
                    return BadRequest($"Ya existe un usuario con email {updateDto.Email} en esta empresa");
                }
            }

            if (updateDto.SucursalId.HasValue && updateDto.SucursalId != usuario.SucursalId)
            {
                var sucursal = await _unitOfWork.Sucursales.GetByIdAsync(updateDto.SucursalId.Value);
                if (sucursal == null)
                {
                    return BadRequest($"La sucursal con ID {updateDto.SucursalId} no existe");
                }
            }

            _mapper.Map(updateDto, usuario);

            if (!string.IsNullOrWhiteSpace(updateDto.Password))
            {
                usuario.PasswordHash = _passwordHashService.HashPassword(updateDto.Password);
            }

            await _unitOfWork.Usuarios.UpdateAsync(usuario);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Usuario actualizado: ID {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar usuario {Id}", id);
            return StatusCode(500, "Error al actualizar usuario");
        }
    }

    // DELETE: api/usuarios/{id} (Soft delete)
    /// <summary>
    /// Desactiva un usuario (soft delete). Solo accesible por AdminGlobal y AdminEmpresa.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "AdminGlobal,AdminEmpresa")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);

            if (usuario == null)
            {
                return NotFound($"Usuario con ID {id} no encontrado");
            }

            usuario.Activo = false;
            usuario.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Usuarios.UpdateAsync(usuario);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Usuario desactivado (soft delete): ID {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar usuario {Id}", id);
            return StatusCode(500, "Error al eliminar usuario");
        }
    }

    // PATCH: api/usuarios/{id}/activar
    /// <summary>
    /// Activa o desactiva un usuario. Solo accesible por AdminGlobal y AdminEmpresa.
    /// </summary>
    [HttpPatch("{id}/activar")]
    [Authorize(Roles = "AdminGlobal,AdminEmpresa")]
    public async Task<IActionResult> ToggleActivo(int id, [FromQuery] bool activo = true)
    {
        try
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);

            if (usuario == null)
            {
                return NotFound($"Usuario con ID {id} no encontrado");
            }

            usuario.Activo = activo;
            usuario.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Usuarios.UpdateAsync(usuario);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Usuario {Estado}: ID {Id}", activo ? "activado" : "desactivado", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar estado de usuario {Id}", id);
            return StatusCode(500, "Error al cambiar estado");
        }
    }
}