namespace Project_Gon.Infrastructure.Mappings;

using AutoMapper;
using Project_Gon.Core.DTOs.MovimientoStock;
using Project_Gon.Core.Entities;

public class MovimientoStockMappingProfile : Profile
{
    public MovimientoStockMappingProfile()
    {
        CreateMap<MovimientoStock, MovimientoStockDto>()
            .ForMember(dest => dest.ProductoId, opt => opt.MapFrom(src => src.Stock.ProductoId))
            .ForMember(dest => dest.ProductoNombre, opt => opt.MapFrom(src => src.Stock.Producto.Nombre))
            .ForMember(dest => dest.SucursalId, opt => opt.MapFrom(src => src.Stock.SucursalId))
            .ForMember(dest => dest.SucursalNombre, opt => opt.MapFrom(src => src.Stock.Sucursal.Nombre))
            .ForMember(dest => dest.UsuarioNombre, opt => opt.MapFrom(src =>
                src.Usuario != null ? src.Usuario.Nombre + " " + src.Usuario.Apellido : null));

        CreateMap<CreateMovimientoStockDto, MovimientoStock>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Stock, opt => opt.Ignore())
            .ForMember(dest => dest.Usuario, opt => opt.Ignore())
            .ForMember(dest => dest.Venta, opt => opt.Ignore())
            .ForMember(dest => dest.Devolucion, opt => opt.Ignore())
            .ForMember(dest => dest.DevolcionId, opt => opt.MapFrom(src => src.DevolucionId));
    }
}