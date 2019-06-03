using ContestPark.Category.API.Infrastructure.Documents;
using ContestPark.Core.CosmosDb.Infrastructure;
using ContestPark.Core.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Category.API.Infrastructure.DataSeed
{
    public class CategoryCollectionSeed : ContextSeedBase<CategoryCollectionSeed>
    {
        public async Task SeedAsync(IServiceProvider service, ILogger<CategoryCollectionSeed> logger)
        {
            var policy = CreatePolicy();

            Service = service;
            Logger = logger;

            await policy.ExecuteAsync(async () =>
            {
                Logger = logger;

                await InsertDataAsync(new List<Documents.Category>
                {
                    new Documents.Category
                    {
                        Order = 0,
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
                             new SubCategory
                            {
                                Order=2,
                                Visibility=true,
                                DisplayPrice="100k",
                                PictuePath="https://static.thenounproject.com/png/14039-200.png",
                                Price=100000,
                                SubCategoryLangs= new List<SubCategoryLang>
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
                            },
                            new SubCategory
                            {
                                Order=1,
                                Visibility=true,
                                DisplayPrice="120k",
                                PictuePath="https://cdn2.iconfinder.com/data/icons/location-map-vehicles/100/Locations-53-512.png",
                                Price=120000,
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
                                Order=1,
                                Visibility=true,
                                DisplayPrice="1k",
                                PictuePath="http://chittagongit.com/images/team-icon-png/team-icon-png-20.jpg",
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
                                Order=3,
                                Visibility=true,
                                DisplayPrice="150k",
                                PictuePath="https://d2n3notmdf08g1.cloudfront.net/common/Login/gotr_icon_whistle.png",
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
            });
        }
    }
}