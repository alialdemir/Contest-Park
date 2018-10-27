using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.MenuItem;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class AccountSettingsViewModel : ViewModelBase<MenuItemList>
    {
        #region Private variables
        

        #endregion Private variables

        #region Constructors

        public AccountSettingsViewModel() 
        {
            Title = ContestParkResources.EditProfile;
        }

        #endregion Constructors

        #region Properties

        public bool IsExit { get; set; }

        #endregion Properties

        #region Methods

        protected override Task InitializeAsync()
        {
            Items.AddRange(new List<MenuItemList>()
            {
                new MenuItemList(ContestParkResources.AccountSettings)
                                {
                                    new Models.MenuItem.MenuItem {
                                        CommandParameter = nameof(LanguageView),
                                        Icon = "fas-user-circle",
                                        Title = ContestParkResources.Language,
                                        MenuType = Enums.MenuTypes.Input
                                    },
                                    new Models.MenuItem.MenuItem {
                                        CommandParameter = "SoundEffects",
                                        Icon = "fas-user-edit",
                                        Title = ContestParkResources.Sounds,
                                        MenuType = Enums.MenuTypes.Input,
                                    },
                                    new Models.MenuItem.MenuItem {
                                        CommandParameter = "Email",
                                        Icon = "fas-envelope",
                                        Title = ContestParkResources.Sounds,
                                        MenuType = Enums.MenuTypes.Input,
                                    },
                                },

                new MenuItemList(ContestParkResources.PictureSettings)
                                {
                                    new Models.MenuItem.MenuItem {
                                        CommandParameter ="Exit",
                                        Icon = "fas-sign-out-alt",
                                        Title = ContestParkResources.LogOut,
                                        MenuType = Enums.MenuTypes.None
                                    },
                                    new Models.MenuItem.MenuItem {
                                        CommandParameter ="Exit",
                                        Icon = "fas-sign-out-alt",
                                        Title = ContestParkResources.LogOut,
                                        MenuType = Enums.MenuTypes.None
                                    },
                                },

                new MenuItemList(ContestParkResources.PasswordChange)
                                {
                                    new Models.MenuItem.MenuItem {
                                        CommandParameter ="Exit",
                                        Icon = "fas-lock",
                                        Title = ContestParkResources.LogOut,
                                        MenuType = Enums.MenuTypes.Input
                                    },
                                    new Models.MenuItem.MenuItem {
                                        CommandParameter ="Exit",
                                        Icon = "fas-unlock",
                                        Title = ContestParkResources.LogOut,
                                        MenuType = Enums.MenuTypes.Input
                                    },
                                },
            });

            return base.InitializeAsync();
        }

        /// <summary>
        /// Parametreden gelen sayfa adına göre işlem yapar
        /// </summary>
        /// <param name="name"></param>
        private async Task ExecutePushPageCommand(string name)
        {
            if (IsExit || IsBusy)
                return;

            IsBusy = true;
             

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        private ICommand _pushPageCommand;

        public ICommand PushPageCommand
        {
            get { return _pushPageCommand ?? (_pushPageCommand = new Command<string>(async (pageName) => await ExecutePushPageCommand(pageName))); }
        }

        #endregion Commands

        #region Navigation

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            IsExit = false;
            base.OnNavigatedTo(parameters);
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            IsExit = true;
            base.OnNavigatedFrom(parameters);
        }

        #endregion Navigation
    }
}