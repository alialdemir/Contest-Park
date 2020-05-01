using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Blocking;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Blocking;
using ContestPark.Mobile.ViewModels.Base;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ContestPark.Mobile.ViewModels
{
    public class BlockingViewModel : ViewModelBase<BlockModel>
    {
        #region Private variables

        private readonly IBlockingService _blockingService;
        private readonly IAnalyticsService _analyticsService;

        #endregion Private variables

        #region Constructor

        public BlockingViewModel(IBlockingService blockingService,
                                 IAnalyticsService analyticsService,
                                 IPageDialogService pageDialogService) : base(dialogService: pageDialogService)
        {
            Title = ContestParkResources.Blocking;
            _blockingService = blockingService;
            _analyticsService = analyticsService;
        }

        #endregion Constructor

        #region Methods

        public override Task InitializeAsync(INavigationParameters parameters = null)
        {
            BlockingListCommand.Execute(null);

            return base.InitializeAsync(parameters);
        }

        /// <summary>
        /// Kullanıcıyu engelle engeli kaldır işlemini yapar
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        private async Task ExecuteBlockingProgressCommand(string userId)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            BlockModel selectedModel = Items.FirstOrDefault(p => p.UserId == userId);
            if (selectedModel != null)
            {
                _analyticsService.SendEvent("Ayarlar", "Kullanıcı Engel Kaldırma", selectedModel.UserName);

                Items.Remove(selectedModel);

                bool isSuccess = await (selectedModel.IsBlocked ?
                    _blockingService.UnBlock(userId) :
                    _blockingService.Block(userId));

                if (!isSuccess)
                {
                    Items.Add(selectedModel);

                    await DisplayAlertAsync("",
                        ContestParkResources.GlobalErrorMessage,
                        ContestParkResources.Okay);
                }
            }

            IsBusy = false;
        }

        /// <summary>
        /// Engellediği kullanıcı listesini getirir
        /// </summary>
        private async Task ExecuteBlockingListCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            ServiceModel = await _blockingService.BlockingList(ServiceModel);

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        private ICommand BlockingListCommand => new CommandAsync(ExecuteBlockingListCommand);

        private ICommand _blockingProgressCommand;

        public ICommand BlockingProgressCommand
        {
            get
            {
                return _blockingProgressCommand ?? (_blockingProgressCommand = new CommandAsync<string>(ExecuteBlockingProgressCommand));
            }
        }

        #endregion Commands
    }
}
