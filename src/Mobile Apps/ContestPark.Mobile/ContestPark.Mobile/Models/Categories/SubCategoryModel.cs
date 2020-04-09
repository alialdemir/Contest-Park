using ContestPark.Mobile.Helpers;

namespace ContestPark.Mobile.Models.Categories
{
    public class SubCategoryModel : BaseModel
    {
        private string displayPrice = "0";
        private string picturePath = "";

        public string DisplayPrice
        {
            get { return displayPrice; }
            set
            {
                displayPrice = value;

                RaisePropertyChanged(() => DisplayPrice);
            }
        }

        private bool _isSubCategoryOpen = false;

        public bool IsSubCategoryOpen
        {
            get { return _isSubCategoryOpen; }
            set
            {
                _isSubCategoryOpen = value;
                RaisePropertyChanged(() => IsSubCategoryOpen);
            }
        }

        public string DefaultLock { get => DefaultImages.DefaultLock; }

        public string PicturePath
        {
            get { return picturePath; }
            set
            {
                picturePath = value;

                RaisePropertyChanged(() => PicturePath);
            }
        }

        public decimal Price { get; set; }

        public short SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
    }
}
