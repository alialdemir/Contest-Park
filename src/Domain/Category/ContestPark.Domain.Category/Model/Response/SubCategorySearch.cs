﻿using ContestPark.Core.Domain;

namespace ContestPark.Domain.Category.Model.Response
{
    public class SubCategorySearch
    {
        public string CategoryName { get; set; }

        public string SubCategoryName { get; set; }

        private string picturePath;

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

        public short SubCategoryId { get; set; }

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