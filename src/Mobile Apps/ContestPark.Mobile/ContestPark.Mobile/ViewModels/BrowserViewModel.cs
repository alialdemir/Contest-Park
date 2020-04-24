using ContestPark.Mobile.ViewModels.Base;
using Prism.Navigation;
using System.Threading.Tasks;

namespace ContestPark.Mobile.ViewModels
{
    public class BrowserViewModel : ViewModelBase
    {
        #region Constructor

        public BrowserViewModel()
        {
        }

        #endregion Constructor

        #region Properties

        private string _link;

        public string Link
        {
            get { return _link; }
            set
            {
                _link = value;
                RaisePropertyChanged(() => Link);
            }
        }

        #endregion Properties

        #region Methods

        public override Task InitializeAsync(INavigationParameters parameters = null)
        {
            if (parameters.ContainsKey("Link"))
                Link = parameters.GetValue<string>("Link");

            return base.InitializeAsync(parameters);
        }
    }

    #endregion Methods
}
