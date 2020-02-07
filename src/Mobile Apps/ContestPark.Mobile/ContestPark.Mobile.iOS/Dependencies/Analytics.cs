using ContestPark.Mobile.Dependencies;
using Foundation;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;

[assembly: Dependency(typeof(ContestPark.Mobile.iOS.Dependencies.Analytics))]

namespace ContestPark.Mobile.iOS.Dependencies
{
    public class Analytics : IAnalytics
    {
        public void SendEvent(string eventCategory, string eventAction, string eventLabel, long? eventValue)
        {
            var additionalData = new Dictionary<string, string>
            {
                { "ec", eventCategory },
                { "ea", eventAction }
            };

            if (!string.IsNullOrEmpty(eventLabel))
            {
                additionalData.Add("el", eventLabel);
            }

            if (eventValue.HasValue)
            {
                additionalData.Add("ev", eventValue.Value.ToString(CultureInfo.InvariantCulture));
            }

            SendEvent("event", additionalData);
        }

        public void SendEvent(string eventId, IDictionary<string, string> parameters)
        {
            if (parameters == null)
            {
                // Firebase.Analytics.Analytics.LogEvent(eventId, (Dictionary<object, object>)null);

                return;
            }

            var keys = new List<NSString>();
            var values = new List<NSString>();

            foreach (var item in parameters)
            {
                keys.Add(new NSString(item.Key));
                values.Add(new NSString(item.Value ?? ""));
            }

            var parametersDictionary = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(values.ToArray(), keys.ToArray(), keys.Count);

            //    Firebase.Analytics.Analytics.LogEvent(eventId, parametersDictionary);
        }

        public void SetUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return;

            //     Firebase.Analytics.Analytics.SetUserId(userId);
        }
    }
}
