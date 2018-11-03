using ContestPark.Core.Domain;

namespace ContestPark.Domain.Category.Model.Response
{
    public class SubCategorySearch
    {
        private string picturePath;
        private int price;
        public string CategoryName { get; set; }

        public string DisplayPrice { get; set; } = "0";

        public string PicturePath
        {
            get { return picturePath; }
            set
            {
                if (DisplayPrice != "0")
                {
                    picturePath = DefaultImages.DefaultLock;
                }
                else picturePath = value;
            }
        }

        public int Price
        {
            get { return price; }
            set
            {
                price = value;
                if (price == 0)
                {
                    DisplayPrice = "0";
                }
            }
        }

        public short SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
    }
}