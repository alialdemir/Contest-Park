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

        /// <summary>
        /// Kullanıcıyu engelle engeli kaldır işlemini yapar
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private void ExecuteBlockProcessingCommand(string userId)
        {
            bool isBlock = Items
                .FirstOrDefault(p => p.UserId == userId)
                .IsBlocked;

            Items
                .FirstOrDefault(p => p.UserId == userId)
                .IsBlocked = !isBlock;

            if (isBlock)
            {
                _blockingService.UnBlock(userId);
            }
            else
            {
                _blockingService.Block(userId);
            }
        }

        #endregion Methods

        #region Commands

        private ICommand _blockProcessingCommand;
        public ICommand BlockProcessingCommand => _blockProcessingCommand ?? (_blockProcessingCommand = new Command<string>((userId) => ExecuteBlockProcessingCommand(userId)));

        #endregion Commands
    }
}