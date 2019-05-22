using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.Blocking;
using ContestPark.Mobile.Services.Blocking;
using ContestPark.Mobile.ViewModels.Base;
using Prism.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class BlockingViewModel : ViewModelBase<BlockModel>
    {
        #region Private variables

        private readonly IBlockingService _blockingService;

        #endregion Private variables

        #region Constructor

        public BlockingViewModel(IBlockingService blockingService,
                                 IPageDialogService pageDialogService) : base(dialogService: pageDialogService)
        {
            Title = ContestParkResources.Blocking;
            _blockingService = blockingService;
        }

        #endregion Constructor

        #region Methods

        protected override async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            ServiceModel = await _blockingService.BlockingList(ServiceModel);

            await base.InitializeAsync();

            IsBusy = false;
        }

        /// <summary>
        /// Kullanıcıyu engelle engeli kaldır işlemini yapar
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task ExecuteBlockingProgressCommand(string userId)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            BlockModel selectedModel = Items.First(p => p.UserId == userId);
            if (selectedModel != null)
            {
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

        #endregion Methods

        #region Commands

        private ICommand _blockingProgressCommand;

        public ICommand BlockingProgressCommand
        {
            get
            {
                return _blockingProgressCommand ?? (_blockingProgressCommand = new Command<string>(async (userId) => await ExecuteBlockingProgressCommand(userId)));
            }
        }

        #endregion Commands
    }
}