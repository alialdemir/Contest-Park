using ContestPark.Mobile.ViewModels.Base;
using Prism.Navigation;

namespace ContestPark.Mobile.ViewModels
{
    public class BrowserViewModel : ViewModelBase
    {
        #region Properties

        private string _link;

        public string Link
        {
            get { return _link; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

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
