using AutoMapper;
using Project_Gon.Core.DTOs.Producto;
using Project_Gon.Core.Entities;

namespace Project_Gon.Infrastructure.Mappings;

/// <summary>
/// Perfil de mapeo para Producto
/// </summary>
public class ProductoMappingProfile : Profile
{
    public ProductoMappingProfile()
    {
        // Producto → ProductoDto
        CreateMap<Producto, ProductoDto>()
            .ForMember(dest => dest.EmpresaNombre, opt => opt.MapFrom(src => src.Empresa!.Nombre))
            .ForMember(dest => dest.CategoriaNombre, opt => opt.MapFrom(src => src.Categoria!.Nombre));

        // CreateProductoDto → Producto
        CreateMap<CreateProductoDto, Producto>();

        // UpdateProductoDto → Producto (mapeo parcial)
        CreateMap<UpdateProductoDto, Producto>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
