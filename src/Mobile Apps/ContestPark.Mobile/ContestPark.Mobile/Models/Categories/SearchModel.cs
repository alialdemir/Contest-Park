using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Helpers;
using Newtonsoft.Json;

namespace ContestPark.Mobile.Models.Categories
{
    public class SearchModel : BaseModel
    {
        private string displayPrice = "0";

        private bool _isFollowing;
        private string picturePath;

        public string CategoryName { get; set; } = "";

        public string DisplayPrice
        {
            get { return displayPrice; }
            set
            {
                displayPrice = value;

                RaisePropertyChanged(() => DisplayPrice);
            }
        }

        public string FullName { get; set; } = "";

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

        public bool IsFollowing
        {
            get { return _isFollowing; }
            set
            {
                _isFollowing = value;

                RaisePropertyChanged(() => IsFollowing);
            }
        }

        public string PicturePath
        {
            get
            {
                return picturePath;
            }
            set
            {
                picturePath = value;

                RaisePropertyChanged(() => PicturePath);
            }
        }

        [JsonIgnore]
        public string ProfilePicturePath
        {
            get { return !string.IsNullOrEmpty(PicturePath) ? PicturePath : DefaultImages.DefaultProfilePicture; }
        }

        public decimal Price { get; set; }

        public SearchTypes SearchType { get; set; }

        public short SubCategoryId { get; set; }
        public string SubCategoryName { get; set; } = "";

        public string UserId { get; set; }
        public string UserName { get; set; } = "";
    }
}
