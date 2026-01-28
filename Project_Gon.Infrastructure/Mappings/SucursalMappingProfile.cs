using AutoMapper;
using Project_Gon.Core.DTOs.Sucursal;
using Project_Gon.Core.Entities;

namespace Project_Gon.Infrastructure.Mappings;

public class SucursalMappingProfile : Profile
{
    public SucursalMappingProfile()
    {
        // Sucursal → SucursalDto
        CreateMap<Sucursal, SucursalDto>()
            .ForMember(dest => dest.EmpresaNombre,
                opt => opt.MapFrom(src => src.Empresa != null ? src.Empresa.Nombre : ""));

        // CreateSucursalDto → Sucursal
        CreateMap<CreateSucursalDto, Sucursal>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.Empresa, opt => opt.Ignore())
            .ForMember(dest => dest.Usuarios, opt => opt.Ignore())
            .ForMember(dest => dest.Stocks, opt => opt.Ignore())
            .ForMember(dest => dest.Ventas, opt => opt.Ignore())
            .ForMember(dest => dest.CajasRegistradoras, opt => opt.Ignore());

        // UpdateSucursalDto → Sucursal
        CreateMap<UpdateSucursalDto, Sucursal>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.EmpresaId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Empresa, opt => opt.Ignore())
            .ForMember(dest => dest.Usuarios, opt => opt.Ignore())
            .ForMember(dest => dest.Stocks, opt => opt.Ignore())
            .ForMember(dest => dest.Ventas, opt => opt.Ignore())
            .ForMember(dest => dest.CajasRegistradoras, opt => opt.Ignore());
    }
}