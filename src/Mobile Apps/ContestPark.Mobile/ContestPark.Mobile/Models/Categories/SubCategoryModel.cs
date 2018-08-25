using Newtonsoft.Json;
using System.ComponentModel;

namespace ContestPark.Mobile.Models.Categories
{
    public class SubCategoryModel : INotifyPropertyChanged
    {
        public string SubCategoryName { get; set; }
        public int SubCategoryId { get; set; }
        private string picturePath = "";

        public string PicturePath
        {
            get { return picturePath; }
            set
            {
                picturePath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PicturePath)));
            }
        }

        private int price;

        public int Price
        {
            get { return price; }
            set
            {
                price = value;
            }
        }

        private string displayPrice = "0";

        public string DisplayPrice
        {
            get { return displayPrice; }
            set
            {
                displayPrice = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayPrice)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [JsonIgnore]
        public bool IsCategoryOpen
        {
            get { return DisplayPrice.Equals("0"); }
        }
    }
}