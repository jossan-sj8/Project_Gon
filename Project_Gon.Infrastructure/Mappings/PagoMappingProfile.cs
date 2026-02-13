namespace Project_Gon.Infrastructure.Mappings;

using AutoMapper;
using Project_Gon.Core.DTOs.Transaccion;
using Project_Gon.Core.Entities;

public class PagoMappingProfile : Profile
{
    public PagoMappingProfile()
    {
        CreateMap<Pago, PagoDto>()
            .ForMember(dest => dest.MetodoPagoNombre, opt => opt.MapFrom(src => src.MetodoPago.Nombre));

        CreateMap<CreatePagoDto, Pago>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Venta, opt => opt.Ignore())
            .ForMember(dest => dest.MetodoPago, opt => opt.Ignore());
    }
}