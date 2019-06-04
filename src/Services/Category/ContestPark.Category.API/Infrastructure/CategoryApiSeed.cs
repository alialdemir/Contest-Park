using ContestPark.Category.API.Infrastructure.Documents;
using ContestPark.Core.CosmosDb.Infrastructure;
using ContestPark.Core.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Category.API.Infrastructure
{
    public class CategoryApiSeed : ContextSeedBase<CategoryApiSeed>
    {
        public async Task SeedAsync(IServiceProvider service, ILogger<CategoryApiSeed> logger)
        {
            var policy = CreatePolicy();

            Service = service;
            Logger = logger;

            await policy.ExecuteAsync(async () =>
            {
                Logger = logger;

                SubCategory referee = new SubCategory
                {
                    DisplayOrder = 2,
                    Visibility = true,
                    DisplayPrice = "120k",
                    PicturePath = "https://static.thenounproject.com/png/14039-200.png",
                    Price = 120000,
                    FollowerCount = 1,
                    SubCategoryLangs = new List<SubCategoryLang>
                                {
                                    new SubCategoryLang
                                    {
                                         LanguageId = Languages.Turkish,
                                         SubCategoryName = "Hakem"
                                    },
                                    new SubCategoryLang
                                    {
                                         LanguageId = Languages.English,
                                         SubCategoryName = "Referee"
                                    },
                                }
                };

                await InsertDataAsync(new List<Documents.Category>
                {
                    new Documents.Category
                    {
                        DisplayOrder = 0,
                        Visibility =true,
                        CategoryLangs= new List<CategoryLang>
                        {
                            new CategoryLang
                            {
                                 CategoryName = "Futbol",
                                 LanguageId = Languages.Turkish
                            },
                            new CategoryLang
                            {
                                 CategoryName="Football",
                                 LanguageId= Languages.English
                            },
                        },
                        SubCategories = new List<SubCategory>
                        {
                            referee,
                            new SubCategory
                            {
                                DisplayOrder=1,
                                Visibility=true,
                                DisplayPrice = "0",
                                Price = 0,
                                PicturePath="https://cdn2.iconfinder.com/data/icons/location-map-vehicles/100/Locations-53-512.png",
                                SubCategoryLangs = new List<SubCategoryLang>
                                {
                                        new SubCategoryLang
                                        {
                                             LanguageId = Languages.Turkish,
                                             SubCategoryName = "Stadyum"
                                        },
                                        new SubCategoryLang
                                        {
                                             LanguageId = Languages.English,
                                             SubCategoryName = "Stadium"
                                        },
                                }
                            },
                            new SubCategory
                            {
                                DisplayOrder=1,
                                Visibility=true,
                                DisplayPrice="1k",
                                PicturePath="http://chittagongit.com/images/team-icon-png/team-icon-png-20.jpg",
                                Price=1000,
                                SubCategoryLangs = new List<SubCategoryLang>
                                {
                                    new SubCategoryLang
                                    {
                                         LanguageId = Languages.Turkish,
                                         SubCategoryName = "Takımlar"
                                    },
                                    new SubCategoryLang
                                    {
                                         LanguageId = Languages.English,
                                         SubCategoryName = "Teams"
                                    },
                                }
                            },
                            new SubCategory
                            {
                                DisplayOrder=3,
                                Visibility=true,
                                DisplayPrice="150k",
                                PicturePath="https://d2n3notmdf08g1.cloudfront.net/common/Login/gotr_icon_whistle.png",
                                Price=150000,
                                SubCategoryLangs = new List<SubCategoryLang>
                                {
                                    new SubCategoryLang
                                    {
                                         LanguageId = Languages.Turkish,
                                         SubCategoryName = "Antrenörler"
                                    },
                                    new SubCategoryLang
                                    {
                                         LanguageId = Languages.English,
                                         SubCategoryName = "Coaches"
                                    }
                                }
                            }
                        }
                    }
                });

                await InsertDataAsync(new List<OpenSubCategory>
                {
                    new OpenSubCategory
                    {
                         SubCategoryId = referee.Id,
                         UserId = "1111-1111-1111-1111"
                    }
                });

                await InsertDataAsync(new List<FollowSubCategory>
                {
                    new FollowSubCategory
                    {
                        UserId = "1111-1111-1111-1111",
                        SubCategoryId= referee.Id,
                    }
                });
            });
        }
    }
}