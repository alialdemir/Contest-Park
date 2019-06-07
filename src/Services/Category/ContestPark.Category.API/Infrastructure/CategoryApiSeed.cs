using ContestPark.Category.API.Infrastructure.Documents;
using ContestPark.Category.API.IntegrationEvents.Events;
using ContestPark.Core.CosmosDb.Infrastructure;
using ContestPark.Core.Enums;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    Description = "açıklama bla bla bla",
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

                var subCategories = new List<Documents.Category>
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
                                Description = "açıklama bla bla bla",
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
                                 Description = "açıklama bla bla bla",
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
                                Description = "açıklama bla bla bla",
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
                            },
                            new SubCategory
                            {
                                DisplayOrder=3,
                                Visibility=true,
                                DisplayPrice="150k",
                                PicturePath="https://d2n3notmdf08g1.cloudfront.net/common/Login/gotr_icon_whistle.png",
                                Description = "tEST bla bla bla",
                                Price=150000,
                                SubCategoryLangs = new List<SubCategoryLang>
                                {
                                    new SubCategoryLang
                                    {
                                         LanguageId = Languages.Turkish,
                                         SubCategoryName = "Hakkılı test"
                                    },
                                    new SubCategoryLang
                                    {
                                         LanguageId = Languages.English,
                                         SubCategoryName = "Reference Test"
                                    }
                                }
                            }
                        }
                    }
                };

                bool isAddedCategories = await InsertDataAsync(subCategories);

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

                if (isAddedCategories)
                {
                    IEventBus eventBus = service.GetService<IEventBus>();

                    foreach (var category in subCategories)// yeni kaydolan kategorileri elasticsearch tarafına yolladık
                    {
                        foreach (var subCategory in category.SubCategories)
                        {
                            var @event = new NewSubCategoryAddedIntegrationEvent(subCategory.DisplayPrice,
                                                                            subCategory.PicturePath,
                                                                            subCategory.Price,
                                                                            subCategory.Id,
                                                                            subCategory.SubCategoryLangs.Select(s => new Model.LanguageModel
                                                                            {
                                                                                Language = s.LanguageId,
                                                                                Name = s.SubCategoryName
                                                                            }).ToList(),
                                                                            category.CategoryLangs.Select(s => new Model.LanguageModel
                                                                            {
                                                                                Language = s.LanguageId,
                                                                                Name = s.CategoryName
                                                                            }).ToList());

                            eventBus.Publish(@event);
                        }
                    }
                }
            });
        }
    }
}