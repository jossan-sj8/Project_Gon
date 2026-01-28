using Project_Gon.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Project_Gon.Core.DTOs.Auth;

public class RegisterDto
{
    [Required(ErrorMessage = "El RUT es obligatorio")]
    public string Rut { get; set; } = null!;

    [EmailAddress(ErrorMessage = "Email inválido")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    public string Apellido { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "La empresa es obligatoria")]
    public int EmpresaId { get; set; }

    public int? SucursalId { get; set; }

    public RolUsuario? Rol { get; set; }  // Por defecto será Vendedor
}