using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Models.Categories.CategoryDetail;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Category
{
    public class CategoryMockServices : ICategoryService
    {
        public Task<ServiceModel<CategoryModel>> CategoryListAsync(PagingModel pagingModel)
        {
            return Task.FromResult(new ServiceModel<CategoryModel>
            {
                Count = 1,
                Items = new List<CategoryModel>
                 {
                    GetCategoryModel(),
                    GetCategoryModel(),
                    GetCategoryModel(),
                    GetCategoryModel(),
                    GetCategoryModel(),
                    GetCategoryModel(),
                 }
            });
        }

        public Task<int> FollowersCountAsync(short subCategoryId)
        {
            return Task.FromResult(1);
        }

        public Task<ServiceModel<SearchModel>> FollowingSearchModelAsync(PagingModel pagingModel)
        {
            return Task.FromResult(new ServiceModel<SearchModel>
            {
                Count = 1,
                Items = new List<SearchModel>
                 {
                     new SearchModel
                     {
                           SubCategoryName ="Bayraklar",
                           Price=100000,
                           SubCategoryId=1,
                           CategoryName="Bayraklar",
                           DisplayPrice="0",
                           PicturePath = DefaultImages.DefaultLock,
                     },
                 }
            });
        }

        public async Task<bool> FollowSubCategoryAsync(short subCategoryId)
        {
            await Task.Delay(3000);

            return true;
        }

        public Task<CategoryDetailModel> GetSubCategoryDetail(short subCategoryId)
        {
            return Task.FromResult(new CategoryDetailModel
            {
                CategoryFollowersCount = 5,
                Description = "Hava an happy day!",
                IsSubCategoryFollowUpStatus = true,
                Level = 33,
                SubCategoryId = subCategoryId.ToString(),
                SubCategoryName = "Football Players",
                SubCategoryPicturePath = DefaultImages.DefaultLock,
            });
        }

        public Task<bool> IsFollowUpStatusAsync(short subCategoryId)
        {
            return Task.FromResult(true);
        }

        public Task<bool> OpenCategoryAsync(short subCategoryId)
        {
            return Task.FromResult(false);
        }

        public Task<ServiceModel<SearchModel>> SearchAsync(string searchText, short subCategoryId, PagingModel pagingModel)
        {
            ServiceModel<SearchModel> items = new ServiceModel<SearchModel>
            {
                Count = 1,
                Items = new List<SearchModel>
                 {
                     new SearchModel
                     {
                           SubCategoryName ="Bayraklar",
                           Price=100000,
                           SubCategoryId=1,
                           CategoryName="Bayraklar",
                           DisplayPrice="0",
                           PicturePath = DefaultImages.DefaultLock,
                           SearchType = Enums.SearchTypes.Category,
                     },
                     new SearchModel
                     {
                           SubCategoryName ="Bayraklar",
                           Price=100000,
                           SubCategoryId=2,
                           CategoryName="Bayraklar",
                           DisplayPrice="100k",
                           PicturePath = DefaultImages.DefaultLock,
                           SearchType = Enums.SearchTypes.Category,
                     },
                     new SearchModel
                     {
                           SubCategoryName ="Bayraklar",
                           Price=100000,
                           SubCategoryId=3,
                           CategoryName="Bayraklar",
                           DisplayPrice="100k",
                           PicturePath = DefaultImages.DefaultLock,
                           SearchType = Enums.SearchTypes.Category,
                     },
                     new SearchModel
                     {
                           SubCategoryName ="Bayraklar",
                           Price=100000,
                           SubCategoryId=4,
                           CategoryName="Bayraklar",
                           DisplayPrice="100k",
                           PicturePath = DefaultImages.DefaultLock,
                           SearchType = Enums.SearchTypes.Category,
                     },
                     new SearchModel
                     {
                           SubCategoryName ="Bayraklar",
                           Price=100000,
                           SubCategoryId=5,
                           CategoryName="Bayraklar",
                           DisplayPrice="100k",
                           PicturePath = DefaultImages.DefaultLock,
                           SearchType = Enums.SearchTypes.Category,
                     },
                     new SearchModel
                     {
                           SubCategoryName ="Bayraklar",
                           Price=100000,
                           SubCategoryId=6,
                           CategoryName="Bayraklar",
                           DisplayPrice="100k",
                           PicturePath = DefaultImages.DefaultLock,
                           SearchType = Enums.SearchTypes.Category,
                     },
                     new SearchModel
                     {
                           SubCategoryName ="Bayraklar",
                           Price=100000,
                           SubCategoryId=7,
                           CategoryName="Bayraklar",
                           DisplayPrice="100k",
                           PicturePath = DefaultImages.DefaultLock,
                           SearchType = Enums.SearchTypes.Category,
                     },
                 }
            };

            searchText = searchText.ToLower();

            items.Items = items
                            .Items
                            .Where(x =>
                                    x.SubCategoryName.ToLower().Contains(searchText) ||
                                    x.CategoryName.ToLower().Contains(searchText) ||
                                    x.FullName.ToLower().Contains(searchText) ||
                                    x.UserName.ToLower().Contains(searchText)
                            )
                            .ToList();

            return Task.FromResult(items);
        }

        public Task<bool> SubCategoryFollowProgcess(short subCategoryId, bool isSubCategoryFollowUpStatus)
        {
            //  await Task.Delay(3000);

            return Task.FromResult(true);
        }

        public async Task<bool> UnFollowSubCategoryAsync(short subCategoryId)
        {
            await Task.Delay(3000);

            return true;
        }

        private CategoryModel GetCategoryModel()
        {
            return new CategoryModel
            {
                CategoryId = 1,
                CategoryName = "Bayraklar",
                SubCategories = new List<SubCategoryModel>
                          {
                              new SubCategoryModel
                              {
                                   DisplayPrice="0",
                                   PicturePath = DefaultImages.DefaultLock,
                                   SubCategoryId=1,
                                   Price=0,
                                   SubCategoryName="Bayraklar"
                              },
                              GetSubCategoryModel(),
                              GetSubCategoryModel(),
                              GetSubCategoryModel(),
                              GetSubCategoryModel(),
                              GetSubCategoryModel(),
                              GetSubCategoryModel(),
                              GetSubCategoryModel(),
                              GetSubCategoryModel(),
                              GetSubCategoryModel(),
                              GetSubCategoryModel(),
                              GetSubCategoryModel(),
                              GetSubCategoryModel(),
                              GetSubCategoryModel(),
                              GetSubCategoryModel(),
                          }.OrderBy(p => p.Price).ToList()
            };
        }

        private SubCategoryModel GetSubCategoryModel()
        {
            Random rnd = new Random();
            int price = rnd.Next(10, 999);

            return new SubCategoryModel
            {
                DisplayPrice = price.ToString() + "K",
                PicturePath = DefaultImages.DefaultLock,
                SubCategoryId = 1,
                Price = price * 10000,
                SubCategoryName = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10)
            };
        }
    }
}