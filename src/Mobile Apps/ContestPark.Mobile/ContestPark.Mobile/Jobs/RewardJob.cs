using Matcha.BackgroundService;
using Microsoft.AppCenter.Analytics;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Jobs
{
    public class RewardJob : IPeriodicTask
    {
        #region Constructor

        public RewardJob(TimeSpan nextRewardTime)
        {
            Interval = TimeSpan.FromSeconds(5); // nextRewardTime;
        }

        #endregion Constructor

        #region Properties

        public TimeSpan Interval { get; set; }
        public bool MyProperty { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// En son aldığı günlük ödülden birsonraki ödülün süresi gelince push notification göndermesi için servere istek atar
        /// </summary>
        public async Task<bool> StartJob()
        {
            try
            {
                HttpClient httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("tr-TR"));

                string tokenType = "Bearer";
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, "eyJhbGciOiJSUzI1NiIsImtpZCI6IjZCN0FDQzUyMDMwNUJGREI0RjcyNTJEQUVCMjE3N0NDMDkxRkFBRTEiLCJ0eXAiOiJKV1QiLCJ4NXQiOiJhM3JNVWdNRnY5dFBjbExhNnlGM3pBa2ZxdUUifQ.eyJuYmYiOjE1ODU4NzgzNDcsImV4cCI6MTU5MzY1NDM0NywiaXNzIjoibnVsbCIsImF1ZCI6WyJudWxsL3Jlc291cmNlcyIsImJhbGFuY2UiLCJjYXRlZ29yeSIsImNoYXQiLCJkdWVsIiwiZm9sbG93IiwiaWRlbnRpdHkiLCJub3RpZmljYXRpb24iLCJwb3N0IiwicXVlc3Rpb24iLCJzaWduYWxyIl0sImNsaWVudF9pZCI6InhhbWFyaW4iLCJzdWIiOiIxMTExLTExMTEtMTExMS0xMTExIiwiYXV0aF90aW1lIjoxNTg1ODc4MzQ3LCJpZHAiOiJsb2NhbCIsInJvbGUiOiJBZG1pbiwgVXNlciIsInByZWZlcnJlZF91c2VybmFtZSI6ImVuZXMxIiwidW5pcXVlX25hbWUiOiJlbmVzMSIsInNjb3BlIjpbImJhbGFuY2UiLCJjYXRlZ29yeSIsImNoYXQiLCJkdWVsIiwiZm9sbG93IiwiaWRlbnRpdHkiLCJub3RpZmljYXRpb24iLCJwb3N0IiwicXVlc3Rpb24iLCJzaWduYWxyIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbInB3ZCJdfQ.kJKY2BOVqHSu6qGwYzfLL73HAh12Z2gn8hOJdd3QYRyjMIQzkqnnG_NDBoMGEFl012lUu6i4ZIJbNkcBDrXT12vMFrb7Wx_UaHsMhTThYHh01CGGqczXKr90ieVw_KF0BP_juhu3SVGZcVjzFMNrPRIMWIPwvPnVlyUCphRyQBqccDwjzIEO_lPaAzVnnCyMGoO6KmINqETHBhq2XAInQAFPTA48ZFY5DagSsVwiP2xNULUJyjNxrbSP2rSV6k0GaEjaPnFm9mpkbwmtGmbAVXOWssPDXlglOXedq4JRriAzkAAthwnjXIcnR41TDQWE04i5qAvcO_ZHwrsm6UM9xw");

                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.contestpark.com/api/v1/Notification/Push/Send?pushNotificationType=1");

                HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage);

                Analytics.TrackEvent(response.StatusCode.ToString());
            }
            catch (Exception ex)
            {
                Analytics.TrackEvent("hata oldu" + ex.Message);
            }

            //var cacheService = new CacheService();
            //var settingsService = new SettingsService();

            //settingsService.RefreshCurrentUser(new Models.User.UserInfoModel()
            //{
            //    Language = Enums.Languages.Turkish
            //});

            //settingsService.AuthAccessToken =

            //var requestProvider = new RequestProvider(new AnalyticsService(), settingsService);
            //var notificationService = new NotificationService(requestProvider, cacheService);

            //await notificationService.PushSendAsync(Enums.PushNotificationTypes.Reward);

            MyProperty = true;

            return true; //return false when you want to stop or trigger only once
        }

        #endregion Methods
    }
}
