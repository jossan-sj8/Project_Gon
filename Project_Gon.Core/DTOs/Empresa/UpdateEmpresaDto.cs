namespace Project_Gon.Core.DTOs.Empresa;

/// <summary>
/// DTO para actualizar una empresa existente.
/// Validaciones manejadas por FluentValidation (UpdateEmpresaDtoValidator).
/// </summary>
public class UpdateEmpresaDto
{
    public string Nombre { get; set; } = null!;
    public string? Rut { get; set; }
    public string? Region { get; set; }
    public string? Ciudad { get; set; }
    public string? Direccion { get; set; }
    public bool Activo { get; set; } = true;
}