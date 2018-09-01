using ContestPark.Domain.Duel.Interfaces;
using ContestPark.Domain.Duel.Model.Request;
using ContestPark.Domain.Signalr.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Orleans;
using System;
using System.Threading.Tasks;

namespace ContestPark.Signalr.API.Hubs
{
    [Authorize]
    public class ContestParkHub : Hub<IContestParkHub>
    {
        #region Private Variables

        private readonly IClusterClient _clusterClient;

        #endregion Private Variables

        #region Constructor

        public ContestParkHub(
            IClusterClient clusterClient
        )
        {
            _clusterClient = clusterClient;
        }

        #endregion Constructor

        private string UserId => Context?.User?.FindFirst("sub")?.Value;

        public override async Task OnConnectedAsync()
        {
            await Clients
               .Client(Context.ConnectionId)
               .GetConnectionId(Context.ConnectionId);

            await Groups.AddToGroupAsync(Context.ConnectionId, UserId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await Clients
                .Client(Context.ConnectionId)
                .RemoveConnectionId(Context.ConnectionId);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, UserId);

            await base.OnDisconnectedAsync(ex);
        }

        public void SaveAnswer(UserAnswer userAnswer)
        {
            if (userAnswer.DuelId <= 0)
                return;

            _clusterClient
                .GetGrain<IGameGrain>(userAnswer.DuelId)
                 .SaveUserAnswerProcess(userAnswer);
        }
    }
}