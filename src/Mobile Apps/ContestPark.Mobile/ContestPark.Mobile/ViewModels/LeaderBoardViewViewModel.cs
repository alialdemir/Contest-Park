using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Ranking;
using ContestPark.Mobile.Services.Score;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class LeaderBoardViewViewModel : ViewModelBase<RankingModel>
    {
        #region Private variables

        private readonly IScoreService _scoreService;

        #endregion Private variables

        #region Constructors

        public LeaderBoardViewViewModel(INavigationService navigationService,
                                        IScoreService scoreService) : base(navigationService)
        {
            _scoreService = scoreService;
            Title = ContestParkResources.LeaderBoard;
        }

        #endregion Constructors

        #region Properties

        private RankModel _timeLeftModel;

        public RankModel Ranks
        {
            get { return _timeLeftModel; }
            set
            {
                _timeLeftModel = value;
                RaisePropertyChanged(() => Ranks);
            }
        }

        #endregion Properties

        #region Methods

        public override Task InitializeAsync(INavigationParameters parameters = null)
        {
            AllTimesCommandAsync.Execute(null);

            return base.InitializeAsync(parameters);
        }

        /// <summary>
        /// Profile sayfasına git
        /// </summary>
        /// <param name="userName">Profili açılacak kullanıcının kullanıcı adı</param>
        private void ExecuteGotoProfilePageCommand(string userName)
        {
            if (IsBusy || string.IsNullOrEmpty(userName))
                return;

            IsBusy = true;

            NavigateToAsync<ProfileView>(new NavigationParameters
                {
                    {"UserName", userName }
                });

            IsBusy = false;
        }

        /// <summary>
        /// Liderler listesini yükler
        /// </summary>
        private async Task ExecuteAllTimesCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            var rank = await _scoreService.AllTimesAsync(ServiceModel);
            if (rank != null)
            {
                if (Ranks == null || Ranks.ContestFinishDate == null || !Items.Any())
                {
                    Ranks = new RankModel
                    {
                        ContestFinishDate = rank.ContestFinishDate
                    };

                    int rankCount = rank.Ranks.Items.Count();
                    if (rankCount >= 1)
                    {
                        Ranks.First = rank.Ranks.Items.ToList()[0];
                    }

                    if (rankCount >= 2)
                    {
                        Ranks.Secound = rank.Ranks.Items.ToList()[1];
                    }

                    if (rankCount >= 3)
                    {
                        Ranks.Third = rank.Ranks.Items.ToList()[2];
                    }

                    if (rankCount >= 4)
                    {
                        rank.Ranks.Items = rank.Ranks.Items.Skip(3).ToList();

                        ServiceModel = rank.Ranks;
                    }

                    if (rankCount <= 4 && rankCount != 0)
                        IsShowEmptyMessage = false;
                }
                else
                {
                    ServiceModel = rank.Ranks;
                }
            }

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        private ICommand AllTimesCommandAsync => new CommandAsync(ExecuteAllTimesCommandAsync);

        private ICommand _gotoProfilePageCommand;

        public ICommand GotoProfilePageCommand =>
            _gotoProfilePageCommand ?? (_gotoProfilePageCommand = new Command<string>(ExecuteGotoProfilePageCommand));

        #endregion Commands
    }
}
