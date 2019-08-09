using Newtonsoft.Json;

namespace ContestPark.Mobile.Models.Categories
{
    public class SubCategoryModel : BaseModel
    {
        private string displayPrice = "0";
        private string picturePath = "";
        private decimal price;

        public string DisplayPrice
        {
            get { return displayPrice; }
            set
            {
                displayPrice = value;

                RaisePropertyChanged(() => DisplayPrice);
            }
        }

        [JsonIgnore]
        public bool IsCategoryOpen
        {
            get { return DisplayPrice.Equals("0"); }
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

        public decimal Price
        {
            get { return price; }
            set
            {
                price = value;
            }
        }

        public short SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
    }
}
