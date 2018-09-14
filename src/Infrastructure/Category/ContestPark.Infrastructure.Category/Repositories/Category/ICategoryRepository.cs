using ContestPark.Core.Dapper;
using ContestPark.Core.Domain.Model;
using ContestPark.Core.Enums;
using ContestPark.Domain.Category.Model.Response;
using ContestPark.Infrastructure.Category.Entities;
using System;

namespace ContestPark.Infrastructure.Category.Repositories.Category
{
    public interface ICategoryRepository : IRepository<CategoryEntity>
    {
        ServiceResponse<Domain.Category.Model.Response.Category> GetCategoryList(string userId, Languages language, Paging pagingModel);

        ServiceResponse<SubCategorySearch> CategorySearch(string userId, Int16 categoryId, Languages language, Paging paging);
    }
}