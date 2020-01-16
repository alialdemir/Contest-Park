using ContestPark.Mobile.Dependencies;
using ContestPark.Mobile.Droid.Dependencies;
using System.Collections.Generic;
using Xamarin.Forms;

[assembly: Dependency(typeof(Analytics))]

namespace ContestPark.Mobile.Droid.Dependencies
{
    public class Analytics : IAnalytics
    {
        public void SendEvent(string eventCategory, string eventAction, string eventLabel, long? eventValue)
        {
            //var additionalData = new Dictionary<string, string>
            //{
            //    { "ec", eventCategory },
            //    { "ea", eventAction }
            //};

            //if (!string.IsNullOrEmpty(eventLabel))
            //{
            //    additionalData.Add("el", eventLabel);
            //}

            //if (eventValue.HasValue)
            //{
            //    additionalData.Add("ev", eventValue.Value.ToString(CultureInfo.InvariantCulture));
            //}

            //SendEvent("event", additionalData);
        }

        public void SendEvent(string eventId, IDictionary<string, string> parameters)
        {
            //var firebaseAnalytics = FirebaseAnalytics.GetInstance(Forms.Context);

            //if (parameters == null)
            //{
            //    firebaseAnalytics.LogEvent(eventId, null);
            //    return;
            //}

            //var bundle = new Bundle();
            //foreach (var param in parameters)
            //{
            //    bundle.PutString(param.Key, param.Value);
            //}

            //firebaseAnalytics.LogEvent(eventId, bundle);
        }

        public void SetUserId(string userId)
        {
            //if (string.IsNullOrEmpty(userId))
            //    return;

            //var firebaseAnalytics = FirebaseAnalytics.GetInstance(Forms.Context);

            //firebaseAnalytics.SetUserId(userId);
        }
    }
}
