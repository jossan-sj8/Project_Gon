namespace Project_Gon.Core.DTOs.Categoria;

/// <summary>
/// DTO para crear una nueva Categoría
/// </summary>
public class CreateCategoriaDto
{
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int EmpresaId { get; set; }
}