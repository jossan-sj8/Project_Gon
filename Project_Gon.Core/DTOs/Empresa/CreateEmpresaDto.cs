namespace Project_Gon.Core.DTOs.Empresa;

/// <summary>
/// DTO para crear una nueva empresa.
/// Validaciones manejadas por FluentValidation (CreateEmpresaDtoValidator).
/// </summary>
public class CreateEmpresaDto
{
    public string Nombre { get; set; } = null!;
    public string? Rut { get; set; }
    public string? Region { get; set; }
    public string? Ciudad { get; set; }
    public string? Direccion { get; set; }
}