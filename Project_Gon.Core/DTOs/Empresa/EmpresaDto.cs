namespace Project_Gon.Core.DTOs.Empresa;

/// <summary>
/// DTO para retornar datos de una empresa.
/// Se utiliza en las respuestas GET.
/// Solo incluye datos públicos/seguros.
/// </summary>
public class EmpresaDto
{
    /// <summary>
    /// ID único de la empresa
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre de la empresa
    /// </summary>
    public string Nombre { get; set; } = null!;

    /// <summary>
    /// RUT o identificación de la empresa
    /// </summary>
    public string? Rut { get; set; }

    /// <summary>
    /// Región donde opera
    /// </summary>
    public string? Region { get; set; }

    /// <summary>
    /// Ciudad principal
    /// </summary>
    public string? Ciudad { get; set; }

    /// <summary>
    /// Dirección de la empresa
    /// </summary>
    public string? Direccion { get; set; }

    /// <summary>
    /// Indica si la empresa está activa
    /// </summary>
    public bool Activo { get; set; }

    /// <summary>
    /// Fecha de creación
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Última actualización
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}