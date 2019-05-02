using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.Identity;
using ContestPark.Mobile.Models.MenuItem;
using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.Services.Media;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using Prism.Services;
using System.Collections.Generic;
using System.IO;
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

        #endregion Private varaibles

        #region Constructors

        public AccountSettingsViewModel(ISettingsService settingsService,
                                        IPageDialogService dialogService,
                                        IIdentityService identityService,
                                        IMediaService mediaService) : base(dialogService: dialogService)
        {
            Title = ContestParkResources.EditProfile;

            _settingsService = settingsService;
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
                                        Icon = "fas-user-circle",
                                        MenuType = Enums.MenuTypes.Input,
                                        Placeholder = ContestParkResources.Fullname,
                                        Text = _settingsService.CurrentUser.FullName,
                                    },
                                    new InputMenuItem {
                                        Icon = "fas-user-edit",
                                        MenuType = Enums.MenuTypes.Input,
                                        Placeholder = ContestParkResources.UserName,
                                        Text = _settingsService.CurrentUser.UserName,
                                    },
                                },

                new MenuItemList(ContestParkResources.PictureSettings)
                                {
                                    new TextMenuItem {
                                        MenuType = Enums.MenuTypes.Label,
                                        Title = ContestParkResources.ChangeProfilePicture,
                                        Icon = _settingsService.CurrentUser.ProfilePicturePath,
                                        SingleTap = new Command(async () => await ChangeProfilePictureAsync())
        },
                                    new TextMenuItem {
                                        MenuType = Enums.MenuTypes.Label,
                                        Title = ContestParkResources.ChangeCoverPicture,
                                        Icon = _settingsService.CurrentUser.CoverPicturePath,
                                        SingleTap = new Command(async () => await ChangeCoverPicture())
                                    },
                                },

                new MenuItemList(ContestParkResources.PasswordChange)
                                {
                                    new InputMenuItem {
                                        Icon = "fas-lock",
                                        IsPassword = true,
                                        MenuType = Enums.MenuTypes.Input,
                                        Placeholder = ContestParkResources.OldPassword,
                                    },
                                    new InputMenuItem {
                                        IsPassword = true,
                                        Icon = "fas-unlock",
                                        MenuType = Enums.MenuTypes.Input,
                                        Placeholder = ContestParkResources.NewPassword,
                                    },
                                },
            });

            return base.InitializeAsync();
        }

        /// <summary>
        /// Kapak resmi değiştir
        /// </summary>
        private async Task ChangeCoverPicture()
        {
            Stream pictureStream = await _mediaService.ShowMediaActionSheet();
            if (pictureStream == null)
                return;

            await _identityService.ChangeCoverPictureAsync(pictureStream);
        }

        /// <summary>
        /// Profil resmi değiştir
        /// </summary>
        private async Task ChangeProfilePictureAsync()
        {
            Stream pictureStream = await _mediaService.ShowMediaActionSheet();
            if (pictureStream == null)
                return;

            await _identityService.ChangeProfilePictureAsync(pictureStream);
        }

        /// <summary>
        /// Kullanıcı bilgilerini güncelle
        /// </summary>
        private async Task ExecuteSaveCommandAsync()
        {
            if (Items == null || Items.Count == 0)
                return;

            await UpdateUserInfoAsync();
            await UpdatePasswordAsync();
        }

        /// <summary>
        /// Şifre değiştir
        /// </summary>
        private async Task UpdatePasswordAsync()
        {
            var menuItems = Items.Select(p => p.MenuItems).ToList().LastOrDefault();

            string oldPassword = ((InputMenuItem)menuItems.FirstOrDefault(p => ((InputMenuItem)p).Placeholder == ContestParkResources.OldPassword)).Text;
            string newPassword = ((InputMenuItem)menuItems.FirstOrDefault(p => ((InputMenuItem)p).Placeholder == ContestParkResources.NewPassword)).Text;

            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
                return;

            bool isSuccess = await _identityService.ChangePasswordAsync(new ChangePasswordModel
            {
                OldPassword = oldPassword,
                NewPassword = newPassword,
            });

            if (isSuccess)
            {
                ((InputMenuItem)menuItems.FirstOrDefault(p => ((InputMenuItem)p).Placeholder == ContestParkResources.OldPassword)).Text = "";
                ((InputMenuItem)menuItems.FirstOrDefault(p => ((InputMenuItem)p).Placeholder == ContestParkResources.NewPassword)).Text = "";

                await DisplayAlertAsync(ContestParkResources.UpdateSuccessful,
                                        ContestParkResources.YourPasswordHasBeenUpdated,
                                        ContestParkResources.Okay);
            }
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
                await _identityService.RefreshTokenAsync();

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