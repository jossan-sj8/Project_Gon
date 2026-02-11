using AutoMapper;
using Project_Gon.Core.DTOs.Venta;
using Project_Gon.Core.Entities;

namespace Project_Gon.Infrastructure.Mappings;

public class VentaMappingProfile : Profile
{
    public VentaMappingProfile()
    {
        // Venta -> VentaDto
        CreateMap<Venta, VentaDto>()
            .ForMember(dest => dest.EmpresaNombre, opt => opt.MapFrom(src => src.Empresa.Nombre))
            .ForMember(dest => dest.SucursalNombre, opt => opt.MapFrom(src => src.Sucursal.Nombre))
            .ForMember(dest => dest.ClienteNombre, opt => opt.MapFrom(src =>
                src.Cliente != null ? src.Cliente.Nombre : null)) // ✅ CORREGIDO: Cliente no tiene Apellido
            .ForMember(dest => dest.ClienteRut, opt => opt.MapFrom(src => src.Cliente != null ? src.Cliente.Rut : null))
            .ForMember(dest => dest.UsuarioNombre, opt => opt.MapFrom(src => $"{src.Usuario.Nombre} {src.Usuario.Apellido}"))
            .ForMember(dest => dest.TipoDescripcion, opt => opt.MapFrom(src => src.Tipo.ToString()))
            .ForMember(dest => dest.EstadoDescripcion, opt => opt.MapFrom(src => src.Estado.ToString()))
            .ForMember(dest => dest.Detalles, opt => opt.MapFrom(src => src.Detalles));

        // CreateVentaDto -> Venta
        CreateMap<CreateVentaDto, Venta>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioId, opt => opt.Ignore()) // Se asigna en el controller
            .ForMember(dest => dest.SubTotal, opt => opt.Ignore()) // Se calcula en el controller
            .ForMember(dest => dest.Iva, opt => opt.Ignore()) // Se calcula en el controller
            .ForMember(dest => dest.Total, opt => opt.Ignore()) // Se calcula en el controller
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => Core.Enums.EstadoVenta.Completada))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Empresa, opt => opt.Ignore())
            .ForMember(dest => dest.Sucursal, opt => opt.Ignore())
            .ForMember(dest => dest.Cliente, opt => opt.Ignore())
            .ForMember(dest => dest.Usuario, opt => opt.Ignore())
            .ForMember(dest => dest.Detalles, opt => opt.Ignore()) // Se manejan en el controller
            .ForMember(dest => dest.Pagos, opt => opt.Ignore())
            .ForMember(dest => dest.Devoluciones, opt => opt.Ignore());

        // UpdateVentaDto -> Venta
        CreateMap<UpdateVentaDto, Venta>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // DetalleVenta -> DetalleVentaDto
        CreateMap<DetalleVenta, DetalleVentaDto>()
            .ForMember(dest => dest.ProductoNombre, opt => opt.MapFrom(src => src.Producto.Nombre))
            .ForMember(dest => dest.ProductoCodigoBarra, opt => opt.MapFrom(src => src.Producto.Sku)); // ✅ CORREGIDO: Producto tiene Sku, no CodigoBarra

        // CreateDetalleVentaDto -> DetalleVenta
        CreateMap<CreateDetalleVentaDto, DetalleVenta>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.VentaId, opt => opt.Ignore())
            .ForMember(dest => dest.PrecioUnitario, opt => opt.Ignore()) // ✅ Se asigna en el controller
            .ForMember(dest => dest.Subtotal, opt => opt.Ignore()) // Se calcula en el controller
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Venta, opt => opt.Ignore())
            .ForMember(dest => dest.Producto, opt => opt.Ignore());
    }
}