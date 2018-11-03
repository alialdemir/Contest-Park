using ContestPark.Core.Domain.Model;
using System;
using System.Collections.Generic;

namespace ContestPark.Domain.Category.Model.Response
{
    public class Category : ModelBase
    {
        public Int16 CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
    }
}