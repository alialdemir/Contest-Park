using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Dependencies;
using ContestPark.Mobile.Models.LatestVersion;
using ContestPark.Mobile.Services.RequestProvider;
using Microsoft.AppCenter.Crashes;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ContestPark.Mobile.Services.LatestVersion
{
    public class LatestVersionService : ILatestVersionService
    {
        #region Private variables

        private readonly IRequestProvider _requestProvider;
        private readonly IPageDialogService _pageDialogService;
        private readonly string _packageName;
        private readonly string _currentVersion;

        private App _app;

        #endregion Private variables

        #region Constructor

        public LatestVersionService(IRequestProvider requestProvider,
                                    IPageDialogService pageDialogService)
        {
            _requestProvider = requestProvider;
            _pageDialogService = pageDialogService;
            _currentVersion = AppInfo.VersionString;
            _packageName = AppInfo.PackageName;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Eğer uygulama güncel değilse alert çıkarım okaye basılırsa store açar ve uygulama güncel değilse uygulamayı kapatır.
        /// </summary>
        public async Task IfNotUsingLatestVersionOpenInStore()
        {
            var isLatest = await IsUsingLatestVersion();

            if (!isLatest)
            {
                var update = await _pageDialogService.DisplayAlertAsync(ContestParkResources.NewVersion,
                                                                        ContestParkResources.ThereIsaNewVersionOfThisAppAvailableWouldYouLikeToUpdateNow,
                                                                        ContestParkResources.Okay,
                                                                        ContestParkResources.Exit);

                if (update)
                {
                    await OpenAppInStore();
                }

                CloseApp();
            }
        }

        /// <summary>
        /// Uygumanın versiyonu store üzerindeki son versiyon mu kontrol eder
        /// </summary>
        /// <returns>Güncel versiyon ise true değilse false</returns>
        private async Task<bool> IsUsingLatestVersion()
        {
            var latestVersion = string.Empty;

            try
            {
                latestVersion = await GetLatestVersionNumber();

                return Version.Parse(latestVersion).CompareTo(Version.Parse(_currentVersion)) <= 0;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>
                {
                    { "BundleVersion", _currentVersion },
                    { "LatestVersion", latestVersion}
                });
            }

            return true;
        }

        /// <summary>
        /// Run time platsforma göre store açar
        /// </summary>
        /// <returns></returns>
        private async Task OpenAppInStore()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                var supportsUri = await Launcher.CanOpenAsync($"market://details?id={_packageName}");
                if (supportsUri)
                    await Launcher.OpenAsync($"market://details?id={_packageName}");
                else await Launcher.OpenAsync($"https://play.google.com/store/apps/details?id={_packageName}");
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                await Launcher.OpenAsync("https://apps.apple.com/tr/app/contestpark/id1492618836");
            }
        }

        /// <summary>
        /// Runtime platforma göre uygulama store güncel versiyonu verir
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetLatestVersionNumber()
        {
            if (Device.RuntimePlatform == Device.iOS)
                _app = await LookupAppIos();
            else if (Device.RuntimePlatform == Device.Android)
                _app = await LookupAppAndroid();

            return _app?.Version;
        }

        /// <summary>
        /// Uygulamayı kapatır
        /// </summary>
        private void CloseApp()
        {
            Xamarin.Forms.DependencyService.Resolve<IDevice>().CloseApp();
        }

        /// <summary>
        /// App center üzerinden google play de en son yayınlanmış uygulamanın versiyon kodunu verir
        /// </summary>
        /// <returns>Uygulamanın play store'daki güncel versiyon bilgileri</returns>
        public async Task<App> LookupAppAndroid()
        {
            App result = new App() { Version = _currentVersion };
            var url = $"https://play.google.com/store/apps/details?id={_packageName}&hl=tr";

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (var handler = new HttpClientHandler())
                {
                    using (var client = new HttpClient(handler))
                    {
                        using (var responseMsg = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead))
                        {
                            if (!responseMsg.IsSuccessStatusCode)
                            {
                                return result;
                            }

                            try
                            {
                                var content = responseMsg.Content == null ? null : await responseMsg.Content.ReadAsStringAsync();

                                var versionMatch = Regex.Match(content, "<div[^>]*>Mevcut Sürüm</div><span[^>]*><div[^>]*><span[^>]*>(.*?)<").Groups[1];

                                if (versionMatch.Success)
                                {
                                    result.Version = versionMatch.Value.Trim();
                                }
                            }
                            catch (Exception ex)
                            {
                                Crashes.TrackError(ex);

                                return result;
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///  itunes üzerinden uygulamanın apple storedaki güncel sürüm bilgisini döndürür
        /// </summary>
        /// <returns>Uygulamanın app store'daki güncel versiyon bilgileri</returns>
        private async Task<App> LookupAppIos()
        {
            try
            {
                var response = await _requestProvider.GetAsync<VersionIosModel>($"https://itunes.apple.com/lookup?bundleId={_packageName}&country=tr");
                if (response.IsSuccess && response.Data != null && response.Data.Results != null && response.Data.Results.Any())
                {
                    return new App
                    {
                        Version = response.Data.Results.FirstOrDefault().Version.Replace("Version ", ""),
                    };
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>
                {
                    { "BundleIdentifier", _packageName },
                });
            }

            return new App
            {
                Version = _currentVersion
            };
        }

        #endregion Methods
    }
}
