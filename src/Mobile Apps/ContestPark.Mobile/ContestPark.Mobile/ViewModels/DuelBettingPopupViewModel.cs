using ContestPark.Mobile.Services.Cp;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using Rg.Plugins.Popup.Contracts;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class DuelBettingPopupViewModel : ViewModelBase
    {
        #region Private variables

        private readonly ICpService _CpService;

        #endregion Private variables

        #region Constructor

        public DuelBettingPopupViewModel(INavigationService navigationService,
                                         ICpService cpService,
                                         IPopupNavigation popupNavigation) : base(navigationService, popupNavigation: popupNavigation)
        {
            _CpService = cpService;
        }

        #endregion Constructor

        #region Properties

        public Int16 SubcategoryId { get; set; }

        public string SubcategoryName { get; set; }

        public string SubCategoryPicturePath { get; set; }

        /// <summary>
        /// Kullanıcının maksimum ne kadar arttırabileceği altın miktarını tutar
        /// </summary>
        private int MinCp { get; set; } = 0;

        /// <summary>
        /// Kullanıcının altın miktarını tutar
        /// </summary>
        private int _userCp = 0;

        public int UserCp
        {
            get { return _userCp; }
            set
            {
                _userCp = value;
                RaisePropertyChanged(() => UserCp);
            }
        }

        private bool _increaseBetIsEnabled;

        public bool IncreaseBetIsEnabled
        {
            get { return _increaseBetIsEnabled; }
            set
            {
                _increaseBetIsEnabled = value;
                RaisePropertyChanged(() => IncreaseBetIsEnabled);
            }
        }

        private bool _reduceBetIsVisible;

        public bool ReduceBetIsVisible
        {
            get { return _reduceBetIsVisible; }
            set
            {
                _reduceBetIsVisible = value;
                RaisePropertyChanged(() => ReduceBetIsVisible);
            }
        }

        #endregion Properties

        #region Methods

        protected override async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            int userCp = await _CpService.GetTotalCpByUserIdAsync();
            if (userCp > 0) UserCp = userCp / 2;

            MinCp = userCp;

            if (UserCp <= 0)
                IncreaseBetIsEnabled = false;//Kullanıcının altını 0 ise azalt buttonu pasif olsun

            IsBusy = false;
        }

        private void ExecuteIncreaseBetCommand()
        {
            decimal total = Math.Round((decimal)((this.MinCp / 100) * 10));
            total = this.UserCp + total;

            if (total >= this.MinCp)
            {
                this.IncreaseBetIsEnabled = false;
                this.ReduceBetIsVisible = true;
                this.UserCp = this.MinCp;
            }
            else
            {
                this.UserCp = (int)total;
                this.IncreaseBetIsEnabled = true;
                this.ReduceBetIsVisible = true;
            }
        }

        private void ExecuteReduceBetCommand()
        {
            decimal total = Math.Round((decimal)((this.MinCp / 100) * 10));
            if (total > 0 && (this.UserCp - total) > total)
            {
                this.UserCp = this.UserCp - (int)total;
                IncreaseBetIsEnabled = true;
            }
            if (!(total > 0 && (this.UserCp - total) > total)) ReduceBetIsVisible = false;
        }

        private void ExecuteDuelStartCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await PushPopupPageAsync(new DuelStartingPopupView()
                {
                    SubcategoryId = SubcategoryId,
                    Bet = UserCp,
                    StandbyMode = DuelStartingPopupViewModel.StandbyModes.On,
                    SubcategoryName = SubcategoryName,
                    SubCategoryPicturePath = SubCategoryPicturePath
                });

                ClosePopupCommand.Execute(null);
            });

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        public ICommand IncreaseBetCommand => new Command(ExecuteIncreaseBetCommand);

        public ICommand ReduceBetCommand => new Command(ExecuteReduceBetCommand);

        public ICommand DuelStartCommand => new Command(ExecuteDuelStartCommand);

        public ICommand ClosePopupCommand { get { return new Command(async () => await RemoveFirstPopupAsync()); } }

        #endregion Commands
    }
}