using ContestPark.Mobile.ViewModels.Base;

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
    }
}