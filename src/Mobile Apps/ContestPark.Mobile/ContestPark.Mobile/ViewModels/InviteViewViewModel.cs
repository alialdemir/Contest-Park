using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class InviteViewViewModel : ViewModelBase
    {
        #region Private variables

        private readonly ISettingsService _settingsService;

        #endregion Private variables

        #region Constructor

        public InviteViewViewModel(ISettingsService settingsService)
        {
            Title = ContestParkResources.Invite;// ContestPark, Knowledge Contest is fun, you should play too. Please write my user "witcher" to the reference when sign up.
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

        #endregion Methods

        #region Commands

        public ICommand ShareCommand
        {
            get => new Command(() => ExecuteShareCommand());
        }

        #endregion Commands
    }
}
