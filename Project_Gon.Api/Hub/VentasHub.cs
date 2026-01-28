using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace Project_Gon.Api.Hubs;

/// <summary>
/// Hub de SignalR para notificaciones en tiempo real de ventas
/// </summary>
[Authorize]
public class VentasHub : Hub
{
    /// <summary>
    /// Evento cuando un cliente se conecta al hub
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        var empresaId = Context.User?.FindFirst("empresaId")?.Value;

        if (!string.IsNullOrEmpty(empresaId))
        {
            // Agregar el usuario al grupo de su empresa
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Empresa_{empresaId}");
        }

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Evento cuando un cliente se desconecta del hub
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var empresaId = Context.User?.FindFirst("empresaId")?.Value;

        if (!string.IsNullOrEmpty(empresaId))
        {
            // Remover el usuario del grupo de su empresa
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Empresa_{empresaId}");
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Unirse a un grupo específico (por ejemplo, sucursal)
    /// </summary>
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).SendAsync("UserJoined", Context.User?.Identity?.Name);
    }

    /// <summary>
    /// Salir de un grupo específico
    /// </summary>
    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).SendAsync("UserLeft", Context.User?.Identity?.Name);
    }

    /// <summary>
    /// Notificar nueva venta a todos los usuarios de la empresa
    /// </summary>
    public async Task NotifyNewSale(object ventaData)
    {
        var empresaId = Context.User?.FindFirst("empresaId")?.Value;

        if (!string.IsNullOrEmpty(empresaId))
        {
            await Clients.Group($"Empresa_{empresaId}")
                .SendAsync("NewSale", ventaData);
        }
    }

    /// <summary>
    /// Notificar actualización de stock
    /// </summary>
    public async Task NotifyStockUpdate(object stockData)
    {
        var empresaId = Context.User?.FindFirst("empresaId")?.Value;

        if (!string.IsNullOrEmpty(empresaId))
        {
            await Clients.Group($"Empresa_{empresaId}")
                .SendAsync("StockUpdated", stockData);
        }
    }
}