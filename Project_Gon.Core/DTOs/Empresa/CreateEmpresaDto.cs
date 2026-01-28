namespace Project_Gon.Core.DTOs.Empresa;

/// <summary>
/// DTO para crear una nueva empresa.
/// Se utiliza en las solicitudes POST.
/// </summary>
public class CreateEmpresaDto
{
    /// <summary>
    /// Nombre de la empresa (requerido)
    /// </summary>
    public string Nombre { get; set; } = null!;

    /// <summary>
    /// RUT o identificación de la empresa (opcional)
    /// </summary>
    public string? Rut { get; set; }

    /// <summary>
    /// Región donde opera (opcional)
    /// </summary>
    public string? Region { get; set; }

    /// <summary>
    /// Ciudad principal (opcional)
    /// </summary>
    public string? Ciudad { get; set; }

    /// <summary>
    /// Dirección de la empresa (opcional)
    /// </summary>
    public string? Direccion { get; set; }
}