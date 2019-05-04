using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Base;
using Newtonsoft.Json;
using System.ComponentModel;

namespace ContestPark.Mobile.Models.Categories
{
    public class SearchModel : IModelBase, INotifyPropertyChanged
    {
        private string displayPrice = "0";

        private bool isFollow;
        private string picturePath;

        public event PropertyChangedEventHandler PropertyChanged;

        public string CategoryName { get; set; } = "";

        public string DisplayPrice
        {
            get { return displayPrice; }
            set
            {
                displayPrice = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayPrice)));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFollow)));
            }
        }

        public string PicturePath
        {
            get { return picturePath; }
            set
            {
                picturePath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PicturePath)));
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