using ContestPark.Core.Dapper;
using ContestPark.Core.Domain.Model;
using ContestPark.Core.Enums;
using ContestPark.Infrastructure.Category.Entities;

namespace ContestPark.Infrastructure.Category.Repositories.Category
{
    public interface ICategoryRepository : IRepository<CategoryEntity>
    {
        ServiceResponse<Domain.Category.Model.Response.Category> GetCategoryList(string userId, Languages language, Paging pagingModel);
    }
}