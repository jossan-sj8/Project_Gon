using AutoMapper;
using Project_Gon.Core.DTOs.Usuario;
using Project_Gon.Core.Entities;

namespace Project_Gon.Infrastructure.Mappings;

/// <summary>
/// Configuración de AutoMapper para la entidad Usuario.
/// Define las conversiones entre Entity y DTOs.
/// </summary>
public class UsuarioMappingProfile : Profile
{
    public UsuarioMappingProfile()
    {
        // Usuario Entity → UsuarioDto (GET)
        CreateMap<Usuario, UsuarioDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Rut, opt => opt.MapFrom(src => src.Rut))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
            .ForMember(dest => dest.Apellido, opt => opt.MapFrom(src => src.Apellido))
            .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => src.Rol))
            .ForMember(dest => dest.EmpresaId, opt => opt.MapFrom(src => src.EmpresaId))
            .ForMember(dest => dest.EmpresaNombre, opt => opt.MapFrom(src => src.Empresa != null ? src.Empresa.Nombre : "N/A"))
            .ForMember(dest => dest.SucursalId, opt => opt.MapFrom(src => src.SucursalId))
            .ForMember(dest => dest.SucursalNombre, opt => opt.MapFrom(src => src.Sucursal != null ? src.Sucursal.Nombre : null))
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => src.Activo))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // CreateUsuarioDto → Usuario Entity (POST)
        CreateMap<CreateUsuarioDto, Usuario>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Rut, opt => opt.MapFrom(src => src.Rut))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
            .ForMember(dest => dest.Apellido, opt => opt.MapFrom(src => src.Apellido))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Se maneja en el controller con PasswordHashService
            .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => src.Rol))
            .ForMember(dest => dest.EmpresaId, opt => opt.MapFrom(src => src.EmpresaId))
            .ForMember(dest => dest.SucursalId, opt => opt.MapFrom(src => src.SucursalId))
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => (DateTime?)null))
            .ForMember(dest => dest.UltimoAcceso, opt => opt.Ignore())
            .ForMember(dest => dest.Empresa, opt => opt.Ignore())
            .ForMember(dest => dest.Sucursal, opt => opt.Ignore())
            .ForMember(dest => dest.MovimientosStock, opt => opt.Ignore())
            .ForMember(dest => dest.Ventas, opt => opt.Ignore())
            .ForMember(dest => dest.ArqueosCaja, opt => opt.Ignore())
            .ForMember(dest => dest.Devoluciones, opt => opt.Ignore())
            .ForMember(dest => dest.AuditLogs, opt => opt.Ignore())
            .ForMember(dest => dest.ModuloAccesos, opt => opt.Ignore());

        // UpdateUsuarioDto → Usuario Entity (PUT)
        CreateMap<UpdateUsuarioDto, Usuario>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Rut, opt => opt.Ignore()) // RUT no se puede cambiar
            .ForMember(dest => dest.Email, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Email)))
            .ForMember(dest => dest.Nombre, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Nombre)))
            .ForMember(dest => dest.Apellido, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Apellido)))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Se maneja en el controller
            .ForMember(dest => dest.Rol, opt => opt.Condition(src => src.Rol.HasValue))
            .ForMember(dest => dest.SucursalId, opt => opt.Condition(src => src.SucursalId.HasValue))
            .ForMember(dest => dest.Activo, opt => opt.Condition(src => src.Activo.HasValue))
            .ForMember(dest => dest.EmpresaId, opt => opt.Ignore()) // Empresa no se puede cambiar
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UltimoAcceso, opt => opt.Ignore())
            .ForMember(dest => dest.Empresa, opt => opt.Ignore())
            .ForMember(dest => dest.Sucursal, opt => opt.Ignore())
            .ForMember(dest => dest.MovimientosStock, opt => opt.Ignore())
            .ForMember(dest => dest.Ventas, opt => opt.Ignore())
            .ForMember(dest => dest.ArqueosCaja, opt => opt.Ignore())
            .ForMember(dest => dest.Devoluciones, opt => opt.Ignore())
            .ForMember(dest => dest.AuditLogs, opt => opt.Ignore())
            .ForMember(dest => dest.ModuloAccesos, opt => opt.Ignore());
    }
}