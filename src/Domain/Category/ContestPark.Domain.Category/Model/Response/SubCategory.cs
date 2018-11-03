using ContestPark.Core.Domain;
using System;

namespace ContestPark.Domain.Category.Model.Response
{
    public class SubCategory
    {
        private int price;
        public string DisplayPrice { get; set; } = "0";
        public string PicturePath { get; set; } = DefaultImages.DefaultLock;

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

        public Int16 SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
    }
}