using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.Blocking;
using ContestPark.Mobile.Services.Blocking;
using ContestPark.Mobile.ViewModels.Base;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class BlockingViewViewModel : ViewModelBase<UserBlocking>
    {
        #region Private variables

        private readonly IBlockingService _blockingService;

        #endregion Private variables

        #region Constructor

        public BlockingViewViewModel(IBlockingService blockingService)
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

            ServiceModel = await _blockingService.UserBlockingList(ServiceModel);

            await base.InitializeAsync();

            IsBusy = false;
        }

        private Task ExecuteBlockProcessingCommand(string userId)
        {
            Items
                .FirstOrDefault(p => p.UserId == userId)
                .IsBlocked = !Items.FirstOrDefault(p => p.UserId == userId)
                .IsBlocked;
            return Task.CompletedTask;
        }

        #endregion Methods

        #region Commands

        private ICommand _blockProcessingCommand;
        public ICommand BlockProcessingCommand => _blockProcessingCommand ?? (_blockProcessingCommand = new Command<string>(async (userId) => await ExecuteBlockProcessingCommand(userId)));

        #endregion Commands
    }
}