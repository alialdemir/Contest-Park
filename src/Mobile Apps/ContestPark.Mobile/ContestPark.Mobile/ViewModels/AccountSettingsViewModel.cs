using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Identity;
using ContestPark.Mobile.Models.MenuItem;
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

        #endregion Private varaibles

        #region Constructors

        public AccountSettingsViewModel(ISettingsService settingsService,
                                        IPageDialogService dialogService,
                                        IEventAggregator eventAggregator,

                                        IIdentityService identityService,
                                        IMediaService mediaService) : base(dialogService: dialogService)
        {
            Title = ContestParkResources.EditProfile;

            _settingsService = settingsService;
            _eventAggregator = eventAggregator;
            _identityService = identityService;
            _mediaService = mediaService;
        }

        #endregion Constructors

        #region Methods

        protected override Task InitializeAsync()
        {
            Items.AddRange(new List<MenuItemList>()
            {
                new MenuItemList(ContestParkResources.AccountSettings)
                                {
                                    new InputMenuItem {
                                        Icon = "settings_edit_profile_name.svg",
                                        MenuType = Enums.MenuTypes.Input,
                                        Placeholder = ContestParkResources.Fullname,
                                        Text = _settingsService.CurrentUser.FullName,
                                    },
                                    new InputMenuItem {
                                        Icon = "settings_edit_profile_username.svg",
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
                                        Icon = "settings_edit_profile_username.svg",
                                        SingleTap = new Command(async () => await ChangeProfilePictureAsync())
        },
                                    new TextMenuItem {
                                        MenuType = Enums.MenuTypes.Label,
                                        Title = ContestParkResources.ChangeCoverPicture,
                                        Icon = "settings_edit_profile_profile_photo.svg",
                                        SingleTap = new Command(async () => await ChangeCoverPicture()),
                                        CornerRadius = new CornerRadius(0,0,8,8)
                                    },
                                },

                //////new MenuItemList(ContestParkResources.PasswordChange)
                //////                {
                //////                    new InputMenuItem {
                //////                        Icon = "fas-lock",
                //////                        IsPassword = true,
                //////                        MenuType = Enums.MenuTypes.Input,
                //////                        Placeholder = ContestParkResources.OldPassword,
                //////                    },
                //////                    new InputMenuItem {
                //////                        IsPassword = true,
                //////                        Icon = "fas-unlock",
                //////                        MenuType = Enums.MenuTypes.Input,
                //////                        Placeholder = ContestParkResources.NewPassword,
                //////                    },
                //////                },
            });

            return base.InitializeAsync();
        }

        /// <summary>
        /// Kapak resmi değiştir
        /// </summary>
        private async Task ChangeCoverPicture()
        {
            var media = await _mediaService.ShowMediaActionSheet();
            if (media == null)
                return;

            await _identityService.ChangeCoverPictureAsync(media);

            // TODO: reflesh menü ve profildeki resimler
        }

        /// <summary>
        /// Profil resmi değiştir
        /// </summary>
        private async Task ChangeProfilePictureAsync()
        {
            var media = await _mediaService.ShowMediaActionSheet();
            if (media == null)
                return;

            await _identityService.ChangeProfilePictureAsync(media);

            // TODO: reflesh menü ve profildeki resimler
        }

        /// <summary>
        /// Kullanıcı bilgilerini güncelle
        /// </summary>
        private async Task ExecuteSaveCommandAsync()
        {
            if (Items == null || Items.Count == 0)
                return;

            await UpdateUserInfoAsync();
            ////await UpdatePasswordAsync();
        }

        /// <summary>
        /// Şifre değiştir
        /// </summary>
        //////private async Task UpdatePasswordAsync()
        //////{
        //////    var menuItems = Items.Select(p => p.MenuItems).ToList().LastOrDefault();

        //////    string oldPassword = ((InputMenuItem)menuItems.FirstOrDefault(p => ((InputMenuItem)p).Placeholder == ContestParkResources.OldPassword)).Text;
        //////    string newPassword = ((InputMenuItem)menuItems.FirstOrDefault(p => ((InputMenuItem)p).Placeholder == ContestParkResources.NewPassword)).Text;

        //////    if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
        //////        return;

        //////    bool isSuccess = await _identityService.ChangePasswordAsync(new ChangePasswordModel
        //////    {
        //////        OldPassword = oldPassword,
        //////        NewPassword = newPassword,
        //////    });

        //////    if (isSuccess)
        //////    {
        //////        ((InputMenuItem)menuItems.FirstOrDefault(p => ((InputMenuItem)p).Placeholder == ContestParkResources.OldPassword)).Text = "";
        //////        ((InputMenuItem)menuItems.FirstOrDefault(p => ((InputMenuItem)p).Placeholder == ContestParkResources.NewPassword)).Text = "";

        //////        await DisplayAlertAsync(ContestParkResources.UpdateSuccessful,
        //////                                ContestParkResources.YourPasswordHasBeenUpdated,
        //////                                ContestParkResources.Okay);
        //////    }
        //////}

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
