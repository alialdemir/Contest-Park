using ContestPark.Core.Domain;
using System;

namespace ContestPark.Domain.Category.Model.Response
{
    public class SubCategory
    {
        public string SubCategoryName { get; set; }

        public Int16 SubCategoryId { get; set; }

        public string PicturePath { get; set; } = DefaultImages.DefaultLock;

        private int price;

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

        public string DisplayPrice { get; set; } = "0";
    }
}