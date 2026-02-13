namespace Project_Gon.Infrastructure.Mappings;

using AutoMapper;
using Project_Gon.Core.DTOs.Terminal;
using Project_Gon.Core.Entities;

public class TerminalMappingProfile : Profile
{
    public TerminalMappingProfile()
    {
        // CajaRegistradora Entity → TerminalDto
        CreateMap<CajaRegistradora, TerminalDto>()
            .ForMember(dest => dest.SucursalNombre, opt => opt.MapFrom(src => src.Sucursal.Nombre));

        // CreateTerminalDto → CajaRegistradora Entity
        CreateMap<CreateTerminalDto, CajaRegistradora>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Sucursal, opt => opt.Ignore())
            .ForMember(dest => dest.Arqueos, opt => opt.Ignore());

        // UpdateTerminalDto → CajaRegistradora Entity
        CreateMap<UpdateTerminalDto, CajaRegistradora>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}