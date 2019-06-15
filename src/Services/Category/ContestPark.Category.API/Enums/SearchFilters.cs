using System.ComponentModel.DataAnnotations;

namespace ContestPark.Category.API
{
    public enum SearchFilters
    {
        [Display(Name = "subCategoryId")]
        SubCategoryId,

        [Display(Name = "categoryId")]
        CategoryId
    }
}