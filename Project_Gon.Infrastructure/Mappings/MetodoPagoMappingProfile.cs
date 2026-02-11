using AutoMapper;
using Project_Gon.Core.DTOs.MetodoPago;
using Project_Gon.Core.Entities;

namespace Project_Gon.Infrastructure.Mappings;

public class MetodoPagoMappingProfile : Profile
{
    public MetodoPagoMappingProfile()
    {
        // MetodoPago -> MetodoPagoDto
        CreateMap<MetodoPago, MetodoPagoDto>();

        // CreateMetodoPagoDto -> MetodoPago
        CreateMap<CreateMetodoPagoDto, MetodoPago>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Pagos, opt => opt.Ignore())
            .ForMember(dest => dest.DetallesArqueo, opt => opt.Ignore());

        // UpdateMetodoPagoDto -> MetodoPago
        CreateMap<UpdateMetodoPagoDto, MetodoPago>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}