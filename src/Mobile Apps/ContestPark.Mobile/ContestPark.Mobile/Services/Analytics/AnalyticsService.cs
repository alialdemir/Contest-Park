﻿using ContestPark.Mobile.Dependencies;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ContestPark.Mobile.Services.Analytics
{
    public class AnalyticsService : IAnalyticsService
    {
        #region Private variables

        private readonly IAnalytics _analytics;

        #endregion Private variables

        #region Constructor

        public AnalyticsService()
        {
            _analytics = DependencyService.Get<IAnalytics>();
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Ga event
        /// </summary>
        /// <param name="eventCategory"></param>
        /// <param name="eventAction"></param>
        /// <param name="eventLabel"></param>
        /// <param name="eventValue"></param>
        public void SendEvent(string eventCategory, string eventAction, string eventLabel, long? eventValue = null)
        {
            _analytics.SendEvent(eventCategory, eventAction, eventLabel, eventValue);
        }

        /// <summary>
        /// Liste olarak ga eventi gönderir
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="parameters"></param>
        public void SendEvent(string eventId, IDictionary<string, string> parameters)
        {
            _analytics.SendEvent(eventId, parameters);
        }

        /// <summary>
        /// GA login olan kullanıcı id set eder
        /// </summary>
        /// <param name="userId"></param>
        public void SetUserId(string userId)
        {
            _analytics.SetUserId(userId);
        }

        #endregion Methods
    }
}
