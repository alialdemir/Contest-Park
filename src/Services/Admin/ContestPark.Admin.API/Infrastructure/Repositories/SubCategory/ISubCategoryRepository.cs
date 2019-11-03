using ContestPark.Admin.API.Model.SubCategory;
using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;

namespace ContestPark.Admin.API.Infrastructure.Repositories.SubCategory
{
    public interface ISubCategoryRepository
    {
        ServiceModel<SubCategoryDropdownModel> GetSubCategories(Languages language, PagingModel paging);
        System.Threading.Tasks.Task<bool> InsertAsync(SubCategoryInsertModel subCategoryInsert);
        System.Threading.Tasks.Task<bool> UpdateAsync(SubCategoryUpdateModel subCategoryUpdate);
    }
}
