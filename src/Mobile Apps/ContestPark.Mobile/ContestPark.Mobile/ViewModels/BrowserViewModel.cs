using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using Prism.Navigation;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    [QueryProperty(nameof(Link), nameof(Link))]
    public class BrowserViewModel : ViewModelBase
    {
        #region Private variables

        private readonly ISettingsService _settingsService;

        #endregion Private variables

        #region Constructor

        public BrowserViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        #endregion Constructor

        #region Properties

        private string _link;

        public string Link
        {
            get { return _link; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                if (value.EndsWith("balancecode.html"))
                    _link = $"{value}?q={_settingsService.AuthAccessToken}";
                else
                    _link = value;

                RaisePropertyChanged(() => Link);
            }
        }

        #endregion Properties

        #region Methods

        public override void Initialize(INavigationParameters parameters = null)
        {
            if (parameters.ContainsKey("Link"))
                Link = parameters.GetValue<string>("Link");

            base.Initialize(parameters);
        }
    }

    #endregion Methods
}
