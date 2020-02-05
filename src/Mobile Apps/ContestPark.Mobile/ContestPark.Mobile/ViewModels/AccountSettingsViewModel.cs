using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Icons;
using ContestPark.Mobile.Models.Identity;
using ContestPark.Mobile.Models.MenuItem;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.Services.Media;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using Prism.Events;
using Prism.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class AccountSettingsViewModel : ViewModelBase<MenuItemList>
    {
        #region Private varaibles

        private readonly IIdentityService _identityService;

        private readonly IMediaService _mediaService;

        /// <summary>
        /// Defines the _settingsService
        /// </summary>
        private readonly ISettingsService _settingsService;

        private readonly IEventAggregator _eventAggregator;
        private readonly IAnalyticsService _analyticsService;

        #endregion Private varaibles

        #region Constructors

        public AccountSettingsViewModel(ISettingsService settingsService,
                                        IPageDialogService dialogService,
                                        IEventAggregator eventAggregator,
                                        IAnalyticsService analyticsService,
                                        IIdentityService identityService,
                                        IMediaService mediaService) : base(dialogService: dialogService)
        {
            Title = ContestParkResources.EditProfile;

            _settingsService = settingsService;
            _eventAggregator = eventAggregator;
            _analyticsService = analyticsService;
            _identityService = identityService;
            _mediaService = mediaService;
        }

        #endregion Constructors

        #region Methods

        protected override Task InitializeAsync()
        {
            ServiceModel.Items = new List<MenuItemList>()
            {
                new MenuItemList(ContestParkResources.AccountSettings)
                                {
                                    new InputMenuItem {
                                        Icon = ContestParkIcon.FullNameSettings,// "settings_edit_profile_name.svg",
                                        MenuType = Enums.MenuTypes.Input,
                                        Placeholder = ContestParkResources.Fullname,
                                        Text = _settingsService.CurrentUser.FullName,
                                    },
                                    new InputMenuItem {
                                        Icon = ContestParkIcon.UsernameSettings,// "settings_edit_profile_username.svg",
                                        MenuType = Enums.MenuTypes.Input,
                                        Placeholder = ContestParkResources.UserName,
                                        Text = _settingsService.CurrentUser.UserName,
                                        CornerRadius = new CornerRadius(0,0,8,8)
                                    },
                                },

                new MenuItemList(ContestParkResources.PictureSettings)
                                {
                                    new TextMenuItem {
                                        MenuType = Enums.MenuTypes.Label,
                                        Title = ContestParkResources.ChangeProfilePicture,
                                        Icon = ContestParkIcon.ProfilePictureSettings,// "settings_edit_profile_username.svg",
                                        SingleTap = new Command(async () => await ChangeProfilePictureAsync())
        },
                                    new TextMenuItem {
                                        MenuType = Enums.MenuTypes.Label,
                                        Title = ContestParkResources.ChangeCoverPicture,
                                        Icon = ContestParkIcon.CoverPictureSettings,// "settings_edit_profile_profile_photo.svg",
                                        SingleTap = new Command(async () => await ChangeCoverPicture()),
                                        CornerRadius = new CornerRadius(0,0,8,8)
                                    },
                                },
            };

            return base.InitializeAsync();
        }

        /// <summary>
        /// Kapak resmi değiştir
        /// </summary>
        private async Task ChangeCoverPicture()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            var media = await _mediaService.ShowMediaActionSheet();
            if (media != null)
            {
                await _identityService.ChangeCoverPictureAsync(media);

                _analyticsService.SendEvent("Ayarlar", "Kapak Resmi Değiştir", media.AnalyticsEventLabel);
            }

            // TODO: reflesh menü ve profildeki resimleri güncelle
            IsBusy = false;
        }

        /// <summary>
        /// Profil resmi değiştir
        /// </summary>
        private async Task ChangeProfilePictureAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            var media = await _mediaService.ShowMediaActionSheet();
            if (media != null)
            {
                await _identityService.ChangeProfilePictureAsync(media);

                _analyticsService.SendEvent("Ayarlar", "Profil Resmi Değiştir", media.AnalyticsEventLabel);
            }

            // TODO: reflesh menü ve profildeki resimleri güncelle

            IsBusy = false;
        }

        /// <summary>
        /// Kullanıcı bilgilerini güncelle
        /// </summary>
        private async Task ExecuteSaveCommandAsync()
        {
            if (Items == null || Items.Count == 0)
                return;

            await UpdateUserInfoAsync();
        }

        /// <summary>
        /// Kullanıcı adı ve ad soyad bilgilerini güncelle
        /// </summary>
        private async Task UpdateUserInfoAsync()
        {
            var menuItems = Items.Select(p => p.MenuItems).ToList().FirstOrDefault();

            string fullName = ((InputMenuItem)menuItems.FirstOrDefault(p => ((InputMenuItem)p).Placeholder == ContestParkResources.Fullname)).Text;
            string userName = ((InputMenuItem)menuItems.FirstOrDefault(p => ((InputMenuItem)p).Placeholder == ContestParkResources.UserName)).Text;

            if (!(!string.IsNullOrEmpty(fullName) &&
                  !string.IsNullOrEmpty(userName) &&
                (_settingsService.CurrentUser.FullName != fullName || _settingsService.CurrentUser.UserName != userName)))
                return;

            bool isSuccess = await _identityService.UpdateUserInfoAsync(new UpdateUserInfoModel
            {
                UserName = userName,
                FullName = fullName,
            });
            if (isSuccess)
            {
                _analyticsService.SendEvent("Ayarlar", "Kullanıcı Bilgileri", $"{_settingsService.CurrentUser.UserName} - {userName}");

                _settingsService.CurrentUser.UserName = userName;
                _settingsService.CurrentUser.FullName = fullName;

                _settingsService.RefreshCurrentUser(_settingsService.CurrentUser);

                _eventAggregator
                    .GetEvent<ChangeUserInfoEvent>()
                    .Publish(_settingsService.CurrentUser);

                await DisplayAlertAsync(ContestParkResources.UpdateSuccessful,
                                        ContestParkResources.YourInformationHasBeenUpdated,
                                        ContestParkResources.Okay);
            }
        }

        #endregion Methods

        #region Commands

        private ICommand _saveCommand;

        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new Command(async () => await ExecuteSaveCommandAsync()));

        #endregion Commands
    }
}
