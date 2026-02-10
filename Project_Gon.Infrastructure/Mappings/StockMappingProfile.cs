using AutoMapper;
using Project_Gon.Core.DTOs.Stock;
using Project_Gon.Core.Entities;

namespace Project_Gon.Infrastructure.Mappings;

/// <summary>
/// Configuración de AutoMapper para la entidad Stock.
/// Define las conversiones entre Entity y DTOs.
/// </summary>
public class StockMappingProfile : Profile
{
    public StockMappingProfile()
    {
        // Stock Entity → StockDto (GET)
        CreateMap<Stock, StockDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ProductoId, opt => opt.MapFrom(src => src.ProductoId))
            .ForMember(dest => dest.ProductoNombre, opt => opt.MapFrom(src => src.Producto.Nombre))
            .ForMember(dest => dest.SucursalId, opt => opt.MapFrom(src => src.SucursalId))
            .ForMember(dest => dest.SucursalNombre, opt => opt.MapFrom(src => src.Sucursal.Nombre))
            .ForMember(dest => dest.Cantidad, opt => opt.MapFrom(src => src.Cantidad))
            .ForMember(dest => dest.StockMinimo, opt => opt.MapFrom(src => src.StockMinimo))
            .ForMember(dest => dest.FechaVencimiento, opt => opt.MapFrom(src => src.FechaVencimiento))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // CreateStockDto → Stock Entity (POST)
        CreateMap<CreateStockDto, Stock>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductoId, opt => opt.MapFrom(src => src.ProductoId))
            .ForMember(dest => dest.SucursalId, opt => opt.MapFrom(src => src.SucursalId))
            .ForMember(dest => dest.Cantidad, opt => opt.MapFrom(src => src.Cantidad))
            .ForMember(dest => dest.StockMinimo, opt => opt.MapFrom(src => src.StockMinimo))
            .ForMember(dest => dest.FechaVencimiento, opt => opt.MapFrom(src => src.FechaVencimiento))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => (DateTime?)null))
            .ForMember(dest => dest.Producto, opt => opt.Ignore())
            .ForMember(dest => dest.Sucursal, opt => opt.Ignore())
            .ForMember(dest => dest.Movimientos, opt => opt.Ignore());

        // UpdateStockDto → Stock Entity (PUT)
        CreateMap<UpdateStockDto, Stock>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Cantidad, opt => opt.Condition(src => src.Cantidad.HasValue))
            .ForMember(dest => dest.StockMinimo, opt => opt.Condition(src => src.StockMinimo.HasValue))
            .ForMember(dest => dest.FechaVencimiento, opt => opt.Condition(src => src.FechaVencimiento.HasValue))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ProductoId, opt => opt.Ignore())
            .ForMember(dest => dest.SucursalId, opt => opt.Ignore())
            .ForMember(dest => dest.Producto, opt => opt.Ignore())
            .ForMember(dest => dest.Sucursal, opt => opt.Ignore())
            .ForMember(dest => dest.Movimientos, opt => opt.Ignore());
    }
}