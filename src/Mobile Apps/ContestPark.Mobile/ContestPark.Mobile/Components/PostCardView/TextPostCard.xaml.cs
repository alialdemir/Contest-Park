﻿using ContestPark.Mobile.Views;
using Prism.Navigation;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components.PostCardView
{
    
    public partial class TextPostCard : ContentView
    {
        #region Private

        private readonly INavigationService _navigationService;
        private bool IsBusy;

        #endregion Private

        #region Constructors

        public TextPostCard(INavigationService navigationService)
        {
            _navigationService = navigationService;
            InitializeComponent();
        }

        #endregion Constructors

        #region Commands

        private Command<string> gotoProfilePageCommand;

        /// <summary>
        /// Go to ProfilePage load command
        /// </summary>
        public Command<string> GotoProfilePageCommand
        {
            get
            {
                return gotoProfilePageCommand ?? (gotoProfilePageCommand = new Command<string>((userName) =>
                {
                    if (IsBusy || string.IsNullOrEmpty(userName))
                        return;

                    IsBusy = true;

                    _navigationService?.NavigateAsync(nameof(ProfileView), new NavigationParameters
                    {
                         { "UserName", userName }
                    });

                    IsBusy = false;
                }));
            }
        }

        #endregion Commands
    }
}