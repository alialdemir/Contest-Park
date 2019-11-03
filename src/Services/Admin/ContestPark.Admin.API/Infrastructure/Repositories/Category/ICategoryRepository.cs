using ContestPark.Admin.API.Model.Category;
using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;

namespace ContestPark.Admin.API.Infrastructure.Repositories.Category
{
    public interface ICategoryRepository
    {
        ServiceModel<CategoryModel> GetCategories(Languages language, PagingModel paging);

        CategoryUpdateModel GetCategoryById(short categoryId);
    }
}
