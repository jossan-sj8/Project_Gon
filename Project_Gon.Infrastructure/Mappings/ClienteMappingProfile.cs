using AutoMapper;
using Project_Gon.Core.DTOs.Cliente;
using Project_Gon.Core.Entities;

namespace Project_Gon.Infrastructure.Mappings;

/// <summary>
/// Perfil de mapeo para Cliente
/// </summary>
public class ClienteMappingProfile : Profile
{
    public ClienteMappingProfile()
    {
        // Cliente → ClienteDto
        CreateMap<Cliente, ClienteDto>()
            .ForMember(dest => dest.EmpresaNombre, opt => opt.MapFrom(src => src.Empresa!.Nombre));

        // CreateClienteDto → Cliente
        CreateMap<CreateClienteDto, Cliente>();

        // UpdateClienteDto → Cliente (mapeo parcial)
        CreateMap<UpdateClienteDto, Cliente>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}