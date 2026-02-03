using AutoMapper;
using Project_Gon.Core.DTOs.Categoria;
using Project_Gon.Core.Entities;

namespace Project_Gon.Infrastructure.Mappings;

/// <summary>
/// Perfil de mapeo para Categoría
/// </summary>
public class CategoriaMappingProfile : Profile
{
    public CategoriaMappingProfile()
    {
        // Categoria → CategoriaDto
        CreateMap<Categoria, CategoriaDto>()
            .ForMember(dest => dest.EmpresaNombre, opt => opt.MapFrom(src => src.Empresa!.Nombre));

        // CreateCategoriaDto → Categoria
        CreateMap<CreateCategoriaDto, Categoria>();

        // UpdateCategoriaDto → Categoria (mapeo parcial)
        CreateMap<UpdateCategoriaDto, Categoria>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}