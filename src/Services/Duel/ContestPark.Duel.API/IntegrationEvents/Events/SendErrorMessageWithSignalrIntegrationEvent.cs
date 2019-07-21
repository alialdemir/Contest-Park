﻿using ContestPark.EventBus.Events;

namespace ContestPark.Duel.API.IntegrationEvents.Events
{
    public class SendErrorMessageWithSignalrIntegrationEvent : IntegrationEvent
    {
        public SendErrorMessageWithSignalrIntegrationEvent(string userId, string message)
        {
            UserId = userId;
            Message = message;
        }

        public string UserId { get; }
        public string Message { get; }
    }
}
