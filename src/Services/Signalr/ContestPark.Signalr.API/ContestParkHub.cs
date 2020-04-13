using ContestPark.EventBus.Abstractions;
using ContestPark.Signalr.API.IntegrationEvents.Events;
using ContestPark.Signalr.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ContestPark.Signalr.API
{
    [Authorize]
    public class ContestParkHub : Hub
    {
        #region Private variables

        private string userId = String.Empty;
        private readonly IEventBus _eventBus;
        private readonly ILogger<ContestParkHub> _logger;

        #endregion Private variables

        #region Methods

        public ContestParkHub(IEventBus eventBus,
                      ILogger<ContestParkHub> logger)
        {
            _eventBus = eventBus;
            _logger = logger;
        }

        #endregion Methods

        #region Properties

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

        #endregion Properties

        #region Methods

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, UserId);

            await Clients.Client(Context.ConnectionId).SendAsync("GetConnectionId", Context.ConnectionId);

            _logger.LogInformation("Kullanıcı id {UserId} signalr connected id {ConnectionId}", UserId, Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, UserId);

            await Clients.Client(Context.ConnectionId).SendAsync("RemoveConnectionId", Context.ConnectionId);

            _logger.LogInformation("Kullanıcı id {UserId} signalr disconnected id {ConnectionId}", UserId, Context.ConnectionId);

            await base.OnDisconnectedAsync(ex);
        }

        /// <summary>
        /// Oyuncunun verdiği cevabı yakalar
        /// </summary>
        /// <param name="userAnswer"></param>
        public void SaveAnswer(UserAnswerModel userAnswer)
        {
            var @event = new UserAnswerIntegrationEvent(userAnswer.DuelId,
                                                        userAnswer.UserId,
                                                        userAnswer.QuestionId,
                                                        userAnswer.Stylish,
                                                        userAnswer.Time,
                                                        userAnswer.BalanceType);

            _eventBus.Publish(@event);
        }

        /// <summary>
        /// Duello için oluşturduğumuz signalr grubundan ayrıldık
        /// </summary>
        /// <param name="duelId">Duello id</param>
        public void LeaveGroup(int duelId)
        {
            if (duelId > 0)
            {
                Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Duel{duelId}");
            }
        }

        #endregion Methods
    }
}
