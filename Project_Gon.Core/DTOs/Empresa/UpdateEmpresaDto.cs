namespace Project_Gon.Core.DTOs.Empresa;

/// <summary>
/// DTO para actualizar una empresa existente.
/// Se utiliza en las solicitudes PUT.
/// </summary>
public class UpdateEmpresaDto
{
    /// <summary>
    /// Nombre de la empresa
    /// </summary>
    public string? Nombre { get; set; }

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
    public bool? Activo { get; set; }
}