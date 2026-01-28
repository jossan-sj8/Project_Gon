using AutoMapper;
using Project_Gon.Core.DTOs.Empresa;
using Project_Gon.Core.Entities;

namespace Project_Gon.Infrastructure.Mappings;

/// <summary>
/// Configuración de AutoMapper para la entidad Empresa.
/// Define las conversiones entre Entity y DTOs.
/// </summary>
public class EmpresaMappingProfile : Profile
{
    public EmpresaMappingProfile()
    {
        // Empresa Entity → EmpresaDto (GET)
        CreateMap<Empresa, EmpresaDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
            .ForMember(dest => dest.Rut, opt => opt.MapFrom(src => src.Rut))
            .ForMember(dest => dest.Region, opt => opt.MapFrom(src => src.Region))
            .ForMember(dest => dest.Ciudad, opt => opt.MapFrom(src => src.Ciudad))
            .ForMember(dest => dest.Direccion, opt => opt.MapFrom(src => src.Direccion))
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => src.Activo))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // CreateEmpresaDto → Empresa Entity (POST)
        CreateMap<CreateEmpresaDto, Empresa>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
            .ForMember(dest => dest.Rut, opt => opt.MapFrom(src => src.Rut))
            .ForMember(dest => dest.Region, opt => opt.MapFrom(src => src.Region))
            .ForMember(dest => dest.Ciudad, opt => opt.MapFrom(src => src.Ciudad))
            .ForMember(dest => dest.Direccion, opt => opt.MapFrom(src => src.Direccion))
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => (DateTime?)null))
            .ForMember(dest => dest.Sucursales, opt => opt.Ignore())
            .ForMember(dest => dest.Usuarios, opt => opt.Ignore())
            .ForMember(dest => dest.Categorias, opt => opt.Ignore())
            .ForMember(dest => dest.Productos, opt => opt.Ignore())
            .ForMember(dest => dest.Clientes, opt => opt.Ignore())
            .ForMember(dest => dest.Ventas, opt => opt.Ignore())
            .ForMember(dest => dest.Proveedores, opt => opt.Ignore());

        // UpdateEmpresaDto → Empresa Entity (PUT)
        CreateMap<UpdateEmpresaDto, Empresa>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Nombre, opt => opt.Condition(src => src.Nombre != null))
            .ForMember(dest => dest.Rut, opt => opt.Condition(src => src.Rut != null))
            .ForMember(dest => dest.Region, opt => opt.Condition(src => src.Region != null))
            .ForMember(dest => dest.Ciudad, opt => opt.Condition(src => src.Ciudad != null))
            .ForMember(dest => dest.Direccion, opt => opt.Condition(src => src.Direccion != null))
            .ForMember(dest => dest.Activo, opt => opt.Condition(src => src.Activo.HasValue))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Sucursales, opt => opt.Ignore())
            .ForMember(dest => dest.Usuarios, opt => opt.Ignore())
            .ForMember(dest => dest.Categorias, opt => opt.Ignore())
            .ForMember(dest => dest.Productos, opt => opt.Ignore())
            .ForMember(dest => dest.Clientes, opt => opt.Ignore())
            .ForMember(dest => dest.Ventas, opt => opt.Ignore())
            .ForMember(dest => dest.Proveedores, opt => opt.Ignore());
    }
}