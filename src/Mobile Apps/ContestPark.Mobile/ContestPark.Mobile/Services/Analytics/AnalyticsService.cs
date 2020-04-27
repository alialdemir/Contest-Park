﻿using ContestPark.Mobile.Dependencies;
using Microsoft.AppCenter.Crashes;
using System;
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
#if !DEBUG
            _analytics = DependencyService.Get<IAnalytics>();
#endif
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
            try
            {
                _analytics?.SendEvent(eventCategory, eventAction, eventLabel, eventValue);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        /// <summary>
        /// Liste olarak ga eventi gönderir
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="parameters"></param>
        public void SendEvent(string eventId, IDictionary<string, string> parameters)
        {
            try
            {
                _analytics?.SendEvent(eventId, parameters);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        /// <summary>
        /// GA login olan kullanıcı id set eder
        /// </summary>
        /// <param name="userId"></param>
        public void SetUserId(string userId)
        {
            try
            {
                _analytics?.SetUserId(userId);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        #endregion Methods
    }
}
