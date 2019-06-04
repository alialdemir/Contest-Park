﻿using System.Collections.Generic;

namespace ContestPark.Mobile.Models.Categories
{
    public class CategoryModel : BaseModel
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<SubCategoryModel> SubCategories { get; set; } = new List<SubCategoryModel>();
    }
}