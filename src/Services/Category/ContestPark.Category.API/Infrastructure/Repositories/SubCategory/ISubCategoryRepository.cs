﻿using ContestPark.Category.API.Models;
using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using System.Collections.Generic;

namespace ContestPark.Category.API.Infrastructure.Repositories.SubCategory
{
    public interface ISubCategoryRepository
    {
        ServiceModel<CategoryModel> GetCategories(string userId, Languages language, PagingModel pagingModel, bool isAllOpen = false);

        ServiceModel<SubCategoryModel> GetFollowedSubCategories(string userId, Languages language, PagingModel pagingModel);

        bool IsSubCategoryFree(short subCategoryId);

        bool IncreasingFollowersCount(short subCategoryId);

        bool DecreasingFollowersCount(short subCategoryId);

        SubCategoryDetailInfoModel GetSubCategoryDetail(short subCategoryId, Languages language, string userId);

        decimal GetSubCategoryPrice(short subCategoryId);

        IEnumerable<SubCategoryModel> LastCategoriesPlayed(string userId, Languages language, PagingModel pagingModel);

        IEnumerable<SubCategoryModel> RecommendedSubcategories(string userId, Languages language);
    }
}
