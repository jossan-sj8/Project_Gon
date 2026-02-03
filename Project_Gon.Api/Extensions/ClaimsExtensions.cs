using System.Security.Claims;

namespace Project_Gon.Api.Extensions;

/// <summary>
/// Extensiones para extraer y validar claims del JWT
/// </summary>
public static class ClaimsExtensions
{
    /// <summary>
    /// Extrae el ID del usuario del JWT
    /// </summary>
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier);
        return int.TryParse(claim?.Value, out var id) ? id : 0;
    }

    /// <summary>
    /// Extrae el rol del usuario del JWT
    /// </summary>
    public static string? GetUserRole(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Role)?.Value;
    }

    /// <summary>
    /// Extrae el ID de empresa del usuario del JWT
    /// </summary>
    public static int GetEmpresaId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst("empresaId");
        return int.TryParse(claim?.Value, out var id) ? id : 0;
    }

    /// <summary>
    /// Extrae el ID de sucursal del usuario del JWT
    /// </summary>
    public static int? GetSucursalId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst("sucursalId");
        if (int.TryParse(claim?.Value, out var id) && id > 0)
        {
            return id;
        }
        return null;
    }
}