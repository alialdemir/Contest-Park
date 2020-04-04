using ContestPark.Mobile.ViewModels.Base;
using Prism.Navigation;

namespace ContestPark.Mobile.ViewModels
{
    public class QuestionExpectedPopupViewModel : ViewModelBase
    {
        #region Properties

        private string _subcategoryName;

        public string SubcategoryName
        {
            get => _subcategoryName;
            set
            {
                _subcategoryName = value;
                RaisePropertyChanged(() => SubcategoryName);
            }
        }

        private string _subCategoryPicturePath;

        public string SubCategoryPicturePath
        {
            get => _subCategoryPicturePath;
            set
            {
                _subCategoryPicturePath = value;
                RaisePropertyChanged(() => SubCategoryPicturePath);
            }
        }

        private byte _roundCount;

        public byte RoundCount
        {
            get => _roundCount;
            set
            {
                _roundCount = value;
                RaisePropertyChanged(() => RoundCount);
            }
        }

        #endregion Properties

        #region Navgation

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("SubcategoryName"))
                SubcategoryName = parameters.GetValue<string>("SubcategoryName");

            if (parameters.ContainsKey("SubCategoryPicturePath"))
                SubCategoryPicturePath = parameters.GetValue<string>("SubCategoryPicturePath");

            if (parameters.ContainsKey("RoundCount"))
                RoundCount = parameters.GetValue<byte>("RoundCount");

            base.OnNavigatedTo(parameters);
        }

        #endregion Navgation
    }
}
