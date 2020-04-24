using ContestPark.Mobile.AppResources;
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

        public override Task InitializeAsync(INavigationParameters parameters = null)
        {
            ServiceModel = new Models.ServiceModel.ServiceModel<TutorialModel>
            {
                Items = new List<TutorialModel>
                {
                    new TutorialModel
                                    {
                                        Title = ContestParkResources.SelectCategory,
                                        Description = ContestParkResources.SelectTheCategoryYouWantToPlayByTheAreaYouAreInterestedIn,
                                        BackgroundGradientEndColor = "#F22E63",
                                        BackgroundGradientStartColor = "#FF6480",
                                        Icon = "select_categories.svg",
                                        ButtonText = ContestParkResources.Next,
                                    },
                    new TutorialModel
                                    {
                                        Title = ContestParkResources.DeterminingThePrize,
                                        Description = ContestParkResources.PayAnEntranceFeeOfHalfThePrizeYouWantToWin,
                                        BackgroundGradientEndColor = "#D596F6",
                                        BackgroundGradientStartColor = "#6BA4F9",
                                        Icon = "choose_prize.svg",
                                        ButtonText = ContestParkResources.Next,
                                    },
                    new TutorialModel
                                    {
                                        Title = ContestParkResources.KnowQuestions,
                                        Description = ContestParkResources.DefeatYourOpponentByGivingTheFastestAndCorrectAnswersToTheQuestions,
                                        BackgroundGradientEndColor = "#FF7569",
                                        BackgroundGradientStartColor = "#FF4E54",
                                        Icon = "question_answer.svg",
                                        ButtonText = ContestParkResources.Next,
                                    },
                    new TutorialModel
                                    {
                                        Title = ContestParkResources.GetTheMoneyAward,
                                        Description = ContestParkResources.WithdrawThePrizeMoneyFromTheApplicationBySendingAnIbanNumber,
                                        BackgroundGradientEndColor = "#FFB347",
                                        BackgroundGradientStartColor = "#FFCC33",
                                        Icon = "earn_money.svg",
                                        ButtonText = ContestParkResources.LetsStart,
                                    },
                    }
            };

            return base.InitializeAsync(parameters);
        }

        #endregion Methods

        #region Commands

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
