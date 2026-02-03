namespace Project_Gon.Core.DTOs.Categoria;

/// <summary>
/// DTO para actualizar una Categoría existente
/// </summary>
public class UpdateCategoriaDto
{
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
}