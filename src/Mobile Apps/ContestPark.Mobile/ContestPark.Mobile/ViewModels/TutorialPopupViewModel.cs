using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.ServiceModel;
using ContestPark.Mobile.Models.Tutorial;
using ContestPark.Mobile.ViewModels.Base;
using Prism.Navigation;
using Rg.Plugins.Popup.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class TutorialPopupViewModel : ViewModelBase<TutorialModel>
    {
        #region Constructor

        public TutorialPopupViewModel(INavigationService navigationService,
                                      IPopupNavigation popupNavigation) : base(navigationService, popupNavigation: popupNavigation)
        {
        }

        #endregion Constructor

        #region Properties

        private int _selectedIndex = 0;

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

        #region Methods

        public override void Initialize(INavigationParameters parameters = null)
        {
            LoadToturialsCommand.Execute(null);

            base.Initialize(parameters);
        }

        /// <summary>
        /// Tutorial listesini yükler
        /// </summary>
        private void ExecuteLoadToturialsCommand()
        {
            ServiceModel = new ServiceModel<TutorialModel>
            {
                Items = new List<TutorialModel>
                {
                    new TutorialModel
                                    {
                                        Title = ContestParkResources.SelectCategory,
                                        Description = ContestParkResources.SelectTheCategoryYouWantToPlayByTheAreaYouAreInterestedIn,
                                        BackgroundGradientEndColor = "#F22E63",
                                        BackgroundGradientStartColor = "#FF6480",
                                        Icon = "resource://ContestPark.Mobile.Common.Images.select_categories.svg?assembly=ContestPark.Mobile",
                                        ButtonText = ContestParkResources.Next,
                                    },
                    new TutorialModel
                                    {
                                        Title = ContestParkResources.DeterminingThePrize,
                                        Description = ContestParkResources.PayAnEntranceFeeOfHalfThePrizeYouWantToWin,
                                        BackgroundGradientEndColor = "#D596F6",
                                        BackgroundGradientStartColor = "#6BA4F9",
                                        Icon = "resource://ContestPark.Mobile.Common.Images.choose_prize.svg?assembly=ContestPark.Mobile",
                                        ButtonText = ContestParkResources.Next,
                                    },
                    new TutorialModel
                                    {
                                        Title = ContestParkResources.KnowQuestions,
                                        Description = ContestParkResources.DefeatYourOpponentByGivingTheFastestAndCorrectAnswersToTheQuestions,
                                        BackgroundGradientEndColor = "#FF7569",
                                        BackgroundGradientStartColor = "#FF4E54",
                                        Icon = "resource://ContestPark.Mobile.Common.Images.question_answer.svg?assembly=ContestPark.Mobile",
                                        ButtonText = ContestParkResources.Next,
                                    },
                    new TutorialModel
                                    {
                                        Title = ContestParkResources.GetTheMoneyAward,
                                        Description = ContestParkResources.WithdrawThePrizeMoneyFromTheApplicationBySendingAnIbanNumber,
                                        BackgroundGradientEndColor = "#FFB347",
                                        BackgroundGradientStartColor = "#FFCC33",
                                        Icon = "resource://ContestPark.Mobile.Common.Images.earn_money.svg?assembly=ContestPark.Mobile",
                                        ButtonText = ContestParkResources.LetsStart,
                                    },
                    }
            };
        }

        #endregion Methods

        #region Commands

        private ICommand LoadToturialsCommand => new Command(ExecuteLoadToturialsCommand);

        private ICommand _nextTutorialCommand;

        public ICommand NextTutorialCommand
        {
            get
            {
                return _nextTutorialCommand ?? (_nextTutorialCommand = new Command(() =>
          {
              if (SelectedIndex < 3)
                  SelectedIndex += 1;
              else
                  GotoBackCommand.Execute(true);
          }));
            }
        }

        #endregion Commands
    }
}
