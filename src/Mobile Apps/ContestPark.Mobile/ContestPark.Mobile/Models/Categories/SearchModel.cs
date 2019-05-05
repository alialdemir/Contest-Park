using ContestPark.Mobile.Enums;
using Newtonsoft.Json;

namespace ContestPark.Mobile.Models.Categories
{
    public class SearchModel : BaseModel
    {
        private string displayPrice = "0";

        private bool isFollow;
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

        [JsonIgnore]
        public bool IsCategoryOpen
        {
            get { return DisplayPrice.Equals("0"); }
        }

        public bool IsFollow
        {
            get { return isFollow; }
            set
            {
                isFollow = value;

                RaisePropertyChanged(() => IsFollow);
            }
        }

        public string PicturePath
        {
            get { return picturePath; }
            set
            {
                picturePath = value;

                RaisePropertyChanged(() => PicturePath);
            }
        }

        public int Price { get; set; }

        public SearchTypes SearchType { get; set; }
        public short SubCategoryId { get; set; }
        public string SubCategoryName { get; set; } = "";

        public string UserId { get; set; }
        public string UserName { get; set; } = "";
    }
}