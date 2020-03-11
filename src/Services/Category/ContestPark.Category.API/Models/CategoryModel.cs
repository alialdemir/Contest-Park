using System.Collections.Generic;

namespace ContestPark.Category.API.Models
{
    public class CategoryModel
    {
        public short CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<SubCategoryModel> SubCategories { get; set; } = new List<SubCategoryModel>();
    }
}
