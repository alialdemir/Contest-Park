﻿using ContestPark.EventBus.Abstractions;
using ContestPark.Signalr.API.IntegrationEvents.Events;
using ContestPark.Signalr.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace ContestPark.Signalr.API
{
    [Authorize]
    public class ContestParkHub : Hub
    {
        public ContestParkHub(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private string userId = String.Empty;
        private readonly IEventBus _eventBus;

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

            await Clients.Client(Context.ConnectionId).SendAsync("GetConnectionId", Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, UserId);

            await Clients.Client(Context.ConnectionId).SendAsync("RemoveConnectionId", Context.ConnectionId);

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
                                                        userAnswer.Time);

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
                Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Duel{duelId.ToString()}");
            }
        }
    }
}
