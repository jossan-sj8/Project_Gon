namespace Project_Gon.Core.DTOs.Categoria;

/// <summary>
/// DTO para lectura de Categoría
/// </summary>
public class CategoriaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Relaciones
    public int EmpresaId { get; set; }
    public string? EmpresaNombre { get; set; }
}