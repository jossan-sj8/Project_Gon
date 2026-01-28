using System.ComponentModel.DataAnnotations;

namespace Project_Gon.Core.DTOs.Sucursal;

/// <summary>
/// DTO para crear una nueva sucursal
/// </summary>
public class CreateSucursalDto
{
    [Required(ErrorMessage = "La empresa es obligatoria")]
    public int EmpresaId { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(255, ErrorMessage = "El nombre no puede exceder 255 caracteres")]
    public string Nombre { get; set; } = null!;

    [MaxLength(100, ErrorMessage = "La región no puede exceder 100 caracteres")]
    public string? Region { get; set; }

    [MaxLength(100, ErrorMessage = "La ciudad no puede exceder 100 caracteres")]
    public string? Ciudad { get; set; }

    [MaxLength(500, ErrorMessage = "La dirección no puede exceder 500 caracteres")]
    public string? Direccion { get; set; }

    [Phone(ErrorMessage = "Formato de teléfono inválido")]
    [MaxLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
    public string? Telefono { get; set; }
}