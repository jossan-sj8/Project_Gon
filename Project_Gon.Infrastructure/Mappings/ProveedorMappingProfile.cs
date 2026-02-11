using AutoMapper;
using Project_Gon.Core.DTOs.Proveedor;
using Project_Gon.Core.Entities;

namespace Project_Gon.Infrastructure.Mappings;

public class ProveedorMappingProfile : Profile
{
    public ProveedorMappingProfile()
    {
        // Proveedor -> ProveedorDto
        CreateMap<Proveedor, ProveedorDto>()
            .ForMember(dest => dest.EmpresaNombre, opt => opt.MapFrom(src => src.Empresa.Nombre));

        // CreateProveedorDto -> Proveedor
        CreateMap<CreateProveedorDto, Proveedor>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Empresa, opt => opt.Ignore())
            .ForMember(dest => dest.PreciosProveedor, opt => opt.Ignore());

        // UpdateProveedorDto -> Proveedor
        CreateMap<UpdateProveedorDto, Proveedor>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}