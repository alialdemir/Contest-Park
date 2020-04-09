using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Models.Categories.CategoryDetail;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.RequestProvider;
using ContestPark.Mobile.Models.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Category
{
    public class CategoryMockServices : ICategoryService
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

        public Task<ServiceModel<CategoryModel>> CategoryListAsync(PagingModel pagingModel, bool isAllOpen = false)
        {
            return Task.FromResult(new ServiceModel<CategoryModel>
            {
                Count = 1,
                Items = new List<CategoryModel>
                 {
                     new CategoryModel
                    {
                        CategoryId = 1,
                        CategoryName = "Takip Ettiğin Kategoriler",
                        SubCategories = new List<SubCategoryModel>
                                  {
                                      new SubCategoryModel
                                      {
                                           DisplayPrice="0",
                                           PicturePath =  "assets/images/referee.png",
                                           SubCategoryId=1,
                                           Price=0,
                                           SubCategoryName="Hakem"
                                      },
                                      new SubCategoryModel
                                      {
                                           DisplayPrice="0",
                                           PicturePath =  "assets/images/headhitting.png",
                                           SubCategoryId=2,
                                           Price=0,
                                           SubCategoryName="Futbolcular"
                                      },
                                      new SubCategoryModel
                                      {
                                           DisplayPrice="0",
                                           PicturePath =  "assets/images/coach.png",
                                           SubCategoryId=3,
                                           Price=0,
                                           SubCategoryName="Antrenörler"
                                      }
                                  }.OrderBy(p => p.Price).ToList()
                    },
                     new CategoryModel
                    {
                        CategoryId = 1,
                        CategoryName = "Futbol",
                        SubCategories = new List<SubCategoryModel>
                                  {
                                      new SubCategoryModel
                                      {
                                           DisplayPrice="0",
                                           PicturePath =  "assets/images/referee.png",
                                           SubCategoryId=1,
                                           Price=0,
                                           SubCategoryName="Hakem"
                                      },
                                      new SubCategoryModel
                                      {
                                           DisplayPrice="0",
                                           PicturePath =  "assets/images/headhitting.png",
                                           SubCategoryId=2,
                                           Price=0,
                                           SubCategoryName="Futbolcular"
                                      },
                                      new SubCategoryModel
                                      {
                                           DisplayPrice="0",
                                           PicturePath =  "assets/images/coach.png",
                                           SubCategoryId=3,
                                           Price=0,
                                           SubCategoryName="Antrenörler"
                                      },
                                      new SubCategoryModel
                                      {
                                           DisplayPrice="10k",
                                           PicturePath =  DefaultImages.DefaultLock,
                                           SubCategoryId=4,
                                           Price=10000,
                                           SubCategoryName="abc"
                                      },
                                      new SubCategoryModel
                                      {
                                           DisplayPrice="112k",
                                           PicturePath =  DefaultImages.DefaultLock,
                                           SubCategoryId=4,
                                           Price=112000,
                                           SubCategoryName="dsadsa"
                                      },
                                      new SubCategoryModel
                                      {
                                           DisplayPrice="15k",
                                           PicturePath =  DefaultImages.DefaultLock,
                                           SubCategoryId=4,
                                           Price=15000,
                                           SubCategoryName="sfafafs"
                                      },
                                  }.OrderBy(p => p.Price).ToList()
                    },
                     new CategoryModel
                    {
                        CategoryId = 1,
                        CategoryName = "Uygulama Logoları",
                        SubCategories = new List<SubCategoryModel>
                                  {
                                      new SubCategoryModel
                                      {
                                           DisplayPrice="0",
                                           PicturePath =  "assets/images/network.png",
                                           SubCategoryId=1,
                                           Price=0,
                                           SubCategoryName="Sosyal"
                                      },
                                      new SubCategoryModel
                                      {
                                           DisplayPrice="0",
                                           PicturePath =  "assets/images/stockmarket.png",
                                           SubCategoryId=2,
                                           Price=0,
                                           SubCategoryName="Aksiyon"
                                      },
                                      new SubCategoryModel
                                      {
                                           DisplayPrice="0",
                                           PicturePath =  "assets/images/creditcard.png",
                                           SubCategoryId=3,
                                           Price=0,
                                           SubCategoryName="Finans"
                                      },
                                      new SubCategoryModel
                                      {
                                           DisplayPrice="0",
                                           PicturePath =  "assets/images/laptop.png",
                                           SubCategoryId=3,
                                           Price=0,
                                           SubCategoryName="Spor"
                                      },
                                  }.OrderBy(p => p.Price).ToList()
                    },
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
                           SubCategoryName ="Takımlar",
                           Price=100000,
                           SubCategoryId=1,
                           CategoryName="Futbol",
                           DisplayPrice="0",
                           PicturePath =  "assets/images/takimlar.png",
                     },
                 }
            });
        }

        public Task<CategoryDetailModel> GetSubCategoryDetail(short subCategoryId)
        {
            return Task.FromResult(new CategoryDetailModel
            {
                FollowerCount = 5,
                Description = "Takımı bil ödülü kazan!",
                IsSubCategoryFollowUpStatus = true,
                Level = 33,
                SubCategoryId = subCategoryId,
                SubCategoryName = "Takımlar",
                PicturePath = "assets/images/headhitting.png",
            });
        }

        public Task<ResponseModel<string>> OpenCategoryAsync(short subCategoryId, BalanceTypes balanceType = BalanceTypes.Gold)
        {
            return Task.FromResult(new ResponseModel<string>() { IsSuccess = true });
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
                         SearchType = SearchTypes.Player,
                         FullName ="Ali Aldemir",
                         PicturePath = DefaultImages.DefaultProfilePicture,
                         UserId ="2222-2222-2222-2222",
                         UserName = "witcherfearless",
                         IsFollowing = true,
                     },
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
                                   PicturePath =  "assets/images/takimlar.png",
                                   SubCategoryId=1,
                                   Price=0,
                                   SubCategoryName="Takımlar"
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

        public void RemoveCategoryListCache(PagingModel pagingModel)
        {
        }
    }
}
