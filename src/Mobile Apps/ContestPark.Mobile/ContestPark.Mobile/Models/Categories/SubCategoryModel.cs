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

        private bool _isCategoryOpen = false;

        public bool IsCategoryOpen
        {
            get { return _isCategoryOpen; }
            set
            {
                _isCategoryOpen = value;
                RaisePropertyChanged(() => IsCategoryOpen);
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
