using ContestPark.Domain.Signalr.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace ContestPark.Signalr.API.Hubs
{
    [Authorize]
    public class ContestParkHub : Hub<IContestParkHub>
    {
        private string UserId => Context?.User?.FindFirst("sub")?.Value;

        public override async Task OnConnectedAsync()
        {
            await Clients
                .Client(Context.ConnectionId)
                .GetConnectionId(Context.ConnectionId);

            await Groups.AddToGroupAsync(Context.ConnectionId, UserId);

            await Groups.AddToGroupAsync(Context.ConnectionId, Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await Clients
                .Client(Context.ConnectionId)
                .RemoveConnectionId(Context.ConnectionId);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, UserId);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.ConnectionId);

            await base.OnDisconnectedAsync(ex);
        }
    }
}