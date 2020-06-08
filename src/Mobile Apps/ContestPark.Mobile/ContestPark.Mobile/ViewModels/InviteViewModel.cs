using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Components.InviteSocialMedia;
using ContestPark.Mobile.Dependencies;
using ContestPark.Mobile.Models.InviteSocialMedia;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using Prism.Navigation;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class InviteViewModel : ViewModelBase
    {
        #region Private variables

        private readonly ISettingsService _settingsService;
        private readonly IAnalyticsService _analyticsService;

        #endregion Private variables

        #region Constructor

        public InviteViewModel(ISettingsService settingsService,
                               IAnalyticsService analyticsService,
                               INavigationService navigationService) : base(navigationService)
        {
            _settingsService = settingsService;
            _analyticsService = analyticsService;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kullanıcı adını sosyal ağlarda paylaşmasını sağlar
        /// </summary>
        private void ExecuteShareCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            Device.BeginInvokeOnMainThread(() =>
            {
                IConvertUIToImage convertUIToImage = DependencyService.Get<IConvertUIToImage>();
                if (convertUIToImage == null)
                {
                    IsBusy = false;
                    return;
                }
                InviteSocialMediaModel inviteData = new InviteSocialMediaModel
                {
                    UserName = _settingsService.CurrentUser.UserName
                };

                ContentView imageView = null;
                if (Device.RuntimePlatform == Device.iOS)
                {
                    imageView = new InviteSocialMediaViewIos
                    {
                        Data = inviteData
                    };
                }
                else if (Device.RuntimePlatform == Device.Android)
                {
                    imageView = new InviteSocialMediaViewAndroid
                    {
                        Data = inviteData
                    };
                }

                if (imageView == null)
                    return;

                string path = convertUIToImage.GetImagePathByPage(imageView);
                if (string.IsNullOrEmpty(path))
                {
                    IsBusy = false;
                    return;
                }

                Share.RequestAsync(new ShareFileRequest
                {
                    Title = string.Format(ContestParkResources.ContestParkKnowledgeContestIsFunYouShoulPlayTooPleaseWriteMyUserToTheReferenceWhenSignUp, _settingsService.CurrentUser.UserName),
                    File = new ShareFile(path),
                });

                _analyticsService.SendEvent("Paylaşma", "Paylaş", _settingsService.CurrentUser.UserId);

                IsBusy = false;
            });
        }

        #endregion Methods

        #region Commands

        public ICommand ShareCommand
        {
            get => new Command(ExecuteShareCommand);
        }

        #endregion Commands
    }
}
