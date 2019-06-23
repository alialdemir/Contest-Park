using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace ContestPark.Signalr.API
{
    [Authorize]
    public class ContestParkHub : Hub
    {
        private string userId = String.Empty;

        /// <summary>
        /// Current user id
        /// </summary>
        public string UserId
        {
            get
            {
                if (string.IsNullOrEmpty(userId))
                {
                    userId = Context.User.FindFirst("sub")?.Value;
                }

                return userId;
            }
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, UserId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, UserId);
            await base.OnDisconnectedAsync(ex);
        }
    }
}