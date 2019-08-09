﻿using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.RequestProvider;
using ContestPark.Mobile.Models.ServiceModel;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Category
{
    public interface ICategoryService
    {
        Task<ServiceModel<CategoryModel>> CategoryListAsync(PagingModel pagingModel);

        Task<Models.Categories.CategoryDetail.CategoryDetailModel> GetSubCategoryDetail(short subCategoryId);

        Task<ResponseModel<string>> OpenCategoryAsync(short subCategoryId, BalanceTypes balanceType = BalanceTypes.Gold);

        Task<ServiceModel<SearchModel>> SearchAsync(string searchText, short categoryId, PagingModel pagingModel);
    }
}
