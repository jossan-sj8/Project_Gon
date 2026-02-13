namespace Project_Gon.Infrastructure.Mappings;

using AutoMapper;
using Project_Gon.Core.DTOs.Devolucion;
using Project_Gon.Core.Entities;

public class DevolucionMappingProfile : Profile
{
    public DevolucionMappingProfile()
    {
        CreateMap<Devolucion, DevolucionDto>()
            .ForMember(dest => dest.UsuarioNombre, opt => opt.MapFrom(src => src.Usuario.Nombre + " " + src.Usuario.Apellido))
            .ForMember(dest => dest.Detalles, opt => opt.MapFrom(src => src.Detalles));

        CreateMap<CreateDevolucionDto, Devolucion>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => Core.Enums.EstadoDevolucion.Pendiente))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Venta, opt => opt.Ignore())
            .ForMember(dest => dest.Usuario, opt => opt.Ignore())
            .ForMember(dest => dest.Detalles, opt => opt.MapFrom(src => src.Detalles));

        CreateMap<UpdateDevolucionDto, Devolucion>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<DetalleDevolucion, DetalleDevolucionDto>()
            .ForMember(dest => dest.ProductoNombre, opt => opt.MapFrom(src => src.Producto.Nombre));

        CreateMap<CreateDetalleDevolucionDto, DetalleDevolucion>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DevolcionId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Devolucion, opt => opt.Ignore())
            .ForMember(dest => dest.Producto, opt => opt.Ignore());
    }
}