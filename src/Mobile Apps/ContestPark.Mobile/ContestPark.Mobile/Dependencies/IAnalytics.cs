using System.Collections.Generic;

namespace ContestPark.Mobile.Dependencies
{
    public interface IAnalytics
    {
        void SendEvent(string eventCategory, string eventAction, string eventLabel, long? eventValue = null);

        void SendEvent(string eventId, IDictionary<string, string> parameters);

        void SetUserId(string userId);
    }
}
