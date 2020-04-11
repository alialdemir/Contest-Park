using ContestPark.Mobile.Models.Picture;
using ContestPark.Mobile.Models.ServiceModel;
using ContestPark.Mobile.ViewModels.Base;
using Prism.Navigation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Mobile.ViewModels
{
    public class PhotoModalViewModel : ViewModelBase<PictureModel>
    {
        #region Constructor

        public PhotoModalViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        #endregion Constructor

        #region Properties

        private int _selectedIndex;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;

                RaisePropertyChanged(() => SelectedIndex);
            }
        }

        #endregion Properties

        #region Nethods

        protected override Task InitializeAsync(INavigationParameters parameters = null)
        {
            if (parameters.ContainsKey("SelectedIndex"))
                SelectedIndex = parameters.GetValue<int>("SelectedIndex");

            if (parameters.ContainsKey("Pictures"))
            {
                ServiceModel = new ServiceModel<PictureModel>
                {
                    Items = parameters.GetValue<List<PictureModel>>("Pictures")
                };
            }

            return base.InitializeAsync(parameters);
        }

        #endregion Nethods
    }
}
