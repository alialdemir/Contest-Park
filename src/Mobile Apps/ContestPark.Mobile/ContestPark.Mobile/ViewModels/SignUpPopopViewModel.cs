using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Login;
using ContestPark.Mobile.ViewModels.Base;
using Prism.Navigation;
using System.Collections.Generic;

namespace ContestPark.Mobile.ViewModels
{
    public class SignUpPopopViewModel : ViewModelBase<SignUpViewModel>
    {
        public SignUpPopopViewModel(INavigationService navigationService) : base(navigationService)
        {
            ServiceModel = new Models.ServiceModel.ServiceModel<SignUpViewModel>
            {
                Items = new List<SignUpViewModel> {
                    new SignUpViewModel
                    {
                        Image = "fullname.jpg".ToResourceImage(),
                        Title = ContestParkResources.WhatShouldWeopleCallYou,
                        Placeholder = ContestParkResources.Fullname,
                        SignUpType = SignUpTypes.Input
                    },
                    new SignUpViewModel
                    {
                        Image = "username.png".ToResourceImage(),
                        Title = ContestParkResources.WhatYouGetYourUsername,
                        Placeholder = ContestParkResources.UserName,
                        IsVisibleReferenceCode = true,
                        SignUpType = SignUpTypes.Input
                    },
                    new SignUpViewModel
                    {
                        SignUpType = SignUpTypes.Categories
                    },
                }
            };
        }

        #region Properties

        private int _selectedIndex;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                RaisePropertyChanged(() => SelectedIndex);
            }
        }

        #endregion Properties
    }
}
