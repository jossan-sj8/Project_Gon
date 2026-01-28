using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Project_Gon.Core.DTOs.Auth;
using Project_Gon.Core.Entities;
using Project_Gon.Core.Enums;
using Project_Gon.Infrastructure.Repositories;
using Project_Gon.Infrastructure.Services;

namespace Project_Gon.Api.Controllers;

/// <summary>
/// Controlador para autenticación y autorización
/// Maneja login con Email o RUT y registro de nuevos usuarios
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUnitOfWork unitOfWork,
        IPasswordHashService passwordHashService,
        IMapper mapper,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHashService = passwordHashService;
        _mapper = mapper;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Login con Email o RUT
    /// POST: api/auth/login
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            _logger.LogInformation("[LOGIN] Intento de login con: {EmailOrRut}", loginDto.EmailOrRut);

            // Buscar usuario por Email o RUT
            Usuario? usuario = await FindUserByEmailOrRut(loginDto.EmailOrRut);

            _logger.LogInformation("[LOGIN] Usuario encontrado: {Found}", usuario != null);

            if (usuario != null)
            {
                _logger.LogInformation("[LOGIN] Usuario ID: {Id}, RUT: {Rut}, Email: {Email}, Activo: {Activo}",
                    usuario.Id, usuario.Rut, usuario.Email, usuario.Activo);
            }

            if (usuario == null || !usuario.Activo)
            {
                _logger.LogWarning("[LOGIN] Login rechazado - Usuario no encontrado o inactivo");
                return Unauthorized("Credenciales inválidas");
            }

            // Verificar contraseña
            _logger.LogInformation("[LOGIN] Verificando contraseña para usuario ID: {Id}", usuario.Id);
            bool passwordValido = _passwordHashService.VerifyPassword(loginDto.Password, usuario.PasswordHash);
            _logger.LogInformation("[LOGIN] Password válido: {Valid}", passwordValido);

            if (!passwordValido)
            {
                _logger.LogWarning("[LOGIN] Login rechazado - Contraseña incorrecta para usuario ID: {Id}", usuario.Id);
                return Unauthorized("Credenciales inválidas");
            }

            // Actualizar último acceso
            usuario.UltimoAcceso = DateTime.UtcNow;
            await _unitOfWork.Usuarios.UpdateAsync(usuario);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("[LOGIN] Login exitoso para usuario ID: {Id}", usuario.Id);

            // Generar JWT token
            var token = GenerateJwtToken(usuario);

            // Crear response
            var response = new LoginResponseDto
            {
                Token = token.Token,
                Expiration = token.Expiration,
                User = new UserInfoDto
                {
                    Id = usuario.Id,
                    Email = usuario.Email,
                    Rut = usuario.Rut,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Rol = usuario.Rol.ToString(),
                    EmpresaId = usuario.EmpresaId,
                    EmpresaNombre = usuario.Empresa?.Nombre ?? "",
                    SucursalId = usuario.SucursalId,
                    SucursalNombre = usuario.Sucursal?.Nombre
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[LOGIN] Error en login: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { mensaje = "Error interno del servidor", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Registrar nuevo usuario
    /// POST: api/auth/register
    /// IMPORTANTE: 
    /// - RUT es OBLIGATORIO
    /// - Email es OPCIONAL
    /// - SucursalId es REQUERIDA para roles Administrador y Vendedor
    /// - SucursalId debe ser NULL para AdminGlobal
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // ===== VALIDACIÓN 1: RUT ES OBLIGATORIO =====
            if (string.IsNullOrWhiteSpace(registerDto.Rut))
                return BadRequest("El RUT es obligatorio");

            // Limpiar y validar RUT
            var rutLimpio = CleanRut(registerDto.Rut);
            _logger.LogInformation("[REGISTER] RUT limpio guardado: {RutLimpio}", rutLimpio);

            // ===== VALIDACIÓN 2: EMPRESA EXISTE =====
            var empresa = await _unitOfWork.Empresas.GetByIdAsync(registerDto.EmpresaId);
            if (empresa == null)
                return BadRequest("Empresa no encontrada");

            // ===== VALIDACIÓN 3: SUCURSAL SEGÚN ROL =====
            var rol = registerDto.Rol ?? RolUsuario.Vendedor;

            if (rol == RolUsuario.AdminGlobal)
            {
                // AdminGlobal NO debe tener sucursal
                if (registerDto.SucursalId.HasValue)
                    return BadRequest("Los usuarios AdminGlobal no deben tener sucursal asignada");
            }
            else
            {
                // Administrador y Vendedor DEBEN tener sucursal
                if (!registerDto.SucursalId.HasValue)
                    return BadRequest($"Los usuarios con rol '{rol}' deben tener una sucursal asignada");

                // Verificar que la sucursal existe
                var sucursal = await _unitOfWork.Sucursales.GetByIdAsync(registerDto.SucursalId.Value);
                if (sucursal == null)
                    return BadRequest("Sucursal no encontrada");

                // Verificar que la sucursal pertenece a la empresa
                if (sucursal.EmpresaId != registerDto.EmpresaId)
                    return BadRequest("La sucursal no pertenece a la empresa especificada");
            }

            // ===== VALIDACIÓN 4: UNICIDAD DE RUT =====
            var existingByRut = await _unitOfWork.Usuarios
                .GetAsync(u => u.Rut == rutLimpio && u.EmpresaId == registerDto.EmpresaId);
            if (existingByRut != null)
                return BadRequest("RUT ya registrado en esta empresa");

            // ===== VALIDACIÓN 5: UNICIDAD DE EMAIL (si se proporcionó) =====
            if (!string.IsNullOrWhiteSpace(registerDto.Email))
            {
                var existingByEmail = await _unitOfWork.Usuarios
                    .GetAsync(u => u.Email == registerDto.Email && u.EmpresaId == registerDto.EmpresaId);
                if (existingByEmail != null)
                    return BadRequest("Email ya registrado en esta empresa");
            }

            // ===== CREAR NUEVO USUARIO =====
            var usuario = new Usuario
            {
                Rut = rutLimpio,  // OBLIGATORIO
                Email = string.IsNullOrWhiteSpace(registerDto.Email) ? null : registerDto.Email,  // OPCIONAL
                Nombre = registerDto.Nombre,
                Apellido = registerDto.Apellido,
                PasswordHash = _passwordHashService.HashPassword(registerDto.Password),
                EmpresaId = registerDto.EmpresaId,
                SucursalId = registerDto.SucursalId,  // null para AdminGlobal, requerido para otros
                Rol = rol,
                Activo = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Usuarios.AddAsync(usuario);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("[REGISTER] Usuario creado - ID: {Id}, RUT: {Rut}, Email: {Email}",
                usuario.Id, usuario.Rut, usuario.Email);

            // Cargar relaciones para el response
            usuario.Empresa = empresa;
            if (usuario.SucursalId.HasValue)
            {
                usuario.Sucursal = await _unitOfWork.Sucursales.GetByIdAsync(usuario.SucursalId.Value);
            }

            // Generar JWT token para login automático
            var token = GenerateJwtToken(usuario);

            // Crear response
            var response = new LoginResponseDto
            {
                Token = token.Token,
                Expiration = token.Expiration,
                User = new UserInfoDto
                {
                    Id = usuario.Id,
                    Email = usuario.Email,
                    Rut = usuario.Rut,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Rol = usuario.Rol.ToString(),
                    EmpresaId = usuario.EmpresaId,
                    EmpresaNombre = empresa.Nombre,
                    SucursalId = usuario.SucursalId,
                    SucursalNombre = usuario.Sucursal?.Nombre
                }
            };

            return CreatedAtAction(nameof(Login), response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[REGISTER] Error al registrar: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { mensaje = "Error al registrar usuario", detalle = ex.Message });
        }
    }

    #region Private Methods

    private async Task<Usuario?> FindUserByEmailOrRut(string emailOrRut)
    {
        Usuario? usuario = null;

        // Auto-detectar si es email o RUT
        if (emailOrRut.Contains("@"))
        {
            _logger.LogInformation("[FIND] Buscando por EMAIL: {Email}", emailOrRut);
            // Es email
            usuario = await _unitOfWork.Usuarios
                .GetAsync(u => u.Email == emailOrRut && u.Activo);
        }
        else
        {
            // Es RUT
            var rutLimpio = CleanRut(emailOrRut);
            _logger.LogInformation("[FIND] Buscando por RUT limpio: {RutLimpio}", rutLimpio);
            usuario = await _unitOfWork.Usuarios
                .GetAsync(u => u.Rut == rutLimpio && u.Activo);
        }

        // Si encontramos el usuario, cargar las relaciones manualmente
        if (usuario != null)
        {
            _logger.LogInformation("[FIND] Usuario encontrado - ID: {Id}, RUT: {Rut}", usuario.Id, usuario.Rut);

            // Cargar empresa
            var empresa = await _unitOfWork.Empresas.GetByIdAsync(usuario.EmpresaId);
            if (empresa != null)
            {
                usuario.Empresa = empresa;
            }

            // Cargar sucursal si tiene
            if (usuario.SucursalId.HasValue)
            {
                var sucursal = await _unitOfWork.Sucursales.GetByIdAsync(usuario.SucursalId.Value);
                if (sucursal != null)
                {
                    usuario.Sucursal = sucursal;
                }
            }
        }
        else
        {
            _logger.LogWarning("[FIND] Usuario NO encontrado");
        }

        return usuario;
    }

    private string CleanRut(string rut)
    {
        // Normalizar RUT: 12.345.678-9 → 12345678-9
        var rutLimpio = rut.Replace(".", "").Replace(" ", "").ToUpper();
        _logger.LogDebug("[CLEAN_RUT] Original: {Original} -> Limpio: {Limpio}", rut, rutLimpio);
        return rutLimpio;
    }

    private (string Token, DateTime Expiration) GenerateJwtToken(Usuario usuario)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET")
            ?? jwtSettings.GetValue<string>("Secret")
            ?? throw new InvalidOperationException("JWT Secret not configured");

        var key = Encoding.UTF8.GetBytes(secretKey);
        var issuer = jwtSettings.GetValue<string>("Issuer") ?? "ProjectGon";
        var audience = jwtSettings.GetValue<string>("Audience") ?? "ProjectGon.Users";
        var expirationMinutes = jwtSettings.GetValue<int>("ExpirationMinutes", 60);

        var expiration = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email ?? ""),
            new Claim("rut", usuario.Rut ?? ""),
            new Claim("empresaId", usuario.EmpresaId.ToString()),
            new Claim("sucursalId", usuario.SucursalId?.ToString() ?? ""),
            new Claim("rol", usuario.Rol.ToString()),
            new Claim("nombre", usuario.Nombre),
            new Claim("apellido", usuario.Apellido),
            new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiration,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var token = tokenHandler.WriteToken(securityToken);

        return (token, expiration);
    }

    #endregion
}