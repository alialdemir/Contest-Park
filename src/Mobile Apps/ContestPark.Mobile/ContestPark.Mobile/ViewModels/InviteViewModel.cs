using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class InviteViewModel : ViewModelBase
    {
        #region Private variables

        private readonly ISettingsService _settingsService;

        #endregion Private variables

        #region Constructor

        public InviteViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kullanıcı adını sosyal ağlarda paylaşmasını sağlar
        /// </summary>
        private void ExecuteShareCommand()
        {
            Share.RequestAsync(new ShareTextRequest
            {
                Text = string.Format(ContestParkResources.ContestParkKnowledgeContestIsFunYouShoulPlayTooPleaseWriteMyUserToTheReferenceWhenSignUp, _settingsService.CurrentUser.UserName),
                Title = "ContestPark",
                Uri = "https://indir.contestpark.com",
            });
        }

        protected override Task InitializeAsync()
        {
            return base.InitializeAsync();
        }

        #endregion Methods

        #region Commands

        public ICommand ShareCommand
        {
            get => new Command(() => ExecuteShareCommand());
        }

        #endregion Commands
    }
}
