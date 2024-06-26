﻿using ContestPark.EventBus.Events;

namespace ContestPark.Identity.API.IntegrationEvents.Events
{
    public class ChangedGameCountIntegrationEvent : IntegrationEvent
    {
        public ChangedGameCountIntegrationEvent(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; }
    }
}
