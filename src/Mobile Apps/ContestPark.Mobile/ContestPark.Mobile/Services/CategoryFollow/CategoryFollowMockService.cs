using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.CategoryFollow
{
    public class CategoryFollowMockService : ICategoryFollowService
    {
        public Task<ServiceModel<SearchModel>> FollowedSubCategoriesAsync(string searchText, PagingModel pagingModel)
        {
            return Task.FromResult(new ServiceModel<SearchModel>
            {
                Count = 3,
                PageNumber = 1,
                PageSize = 1,
                Items = new List<SearchModel>
                {
                    new SearchModel
                    {
                            DisplayPrice="0",
                            PicturePath=DefaultImages.DefaultLock,
                            Price=0,
                            SubCategoryId= 2,
                            SubCategoryName="Football Players",
                            CategoryName = "Football",
                    },
                    new SearchModel
                    {
                            DisplayPrice="10k",
                            PicturePath=DefaultImages.DefaultLock,
                            Price=10000,
                            SubCategoryId= 3,
                            SubCategoryName="Stadiums",
                            CategoryName = "Football",
                    },
                    new SearchModel
                    {
                            DisplayPrice="100k",
                            PicturePath=DefaultImages.DefaultLock,
                            Price=100000,
                            SubCategoryId=1,
                            SubCategoryName="Teams",
                            CategoryName = "Football",
                    },
                }
            });
        }

        public Task<bool> FollowSubCategoryAsync(short subCategoryId)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsFollowUpStatusAsync(short subCategoryId)
        {
            return Task.FromResult(true);
        }

        public Task<bool> SubCategoryFollowProgcess(short subCategoryId, bool isSubCategoryFollowUpStatus)
        {
            return Task.FromResult(true);
        }

        public Task<bool> UnFollowSubCategoryAsync(short subCategoryId)
        {
            return Task.FromResult(true);
        }
    }
}