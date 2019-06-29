using ContestPark.Category.API.Infrastructure.Documents;
using ContestPark.Category.API.Infrastructure.Repositories.Search;
using ContestPark.Category.API.IntegrationEvents.EventHandling;
using ContestPark.Category.API.IntegrationEvents.Events;
using ContestPark.Core.CosmosDb.Infrastructure;
using ContestPark.Core.Enums;
using ContestPark.Core.Models;
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
                SubCategory referee = new SubCategory
                {
                    Id = "9d15a162-9ffc-42aa-91dc-d7f02b6f0080",
                    DisplayOrder = 2,
                    Visibility = true,
                    DisplayPrice = "120k",
                    PicturePath = "https://static.thenounproject.com/png/14039-200.png",
                    Price = 120000,
                    FollowerCount = 1,
                    Description = "açıklama bla bla bla",
                    SubCategoryLocalized = new List<Localized>
                                {
                                    new Localized
                                    {
                                         Language = Languages.Turkish,
                                         Text = "Hakem"
                                    },
                                    new Localized
                                    {
                                         Language = Languages.English,
                                         Text = "Referee"
                                    },
                                }
                };

                var subCategories = new List<Documents.Category>
                {
                    new Documents.Category
                    {
                        Id = "bb6d7a05-7801-4e97-b7fc-e607f2c89b09",
                        DisplayOrder = 0,
                        Visibility =true,
                        CategoryLocalized= new List<Localized>
                        {
                            new Localized
                            {
                                 Text = "Futbol",
                                 Language = Languages.Turkish
                            },
                            new Localized
                            {
                                 Text="Football",
                                 Language= Languages.English
                            },
                        },
                        SubCategories = new List<SubCategory>
                        {
                            referee,
                            new SubCategory
                            {
                                Id ="7c3a26b7-74df-4128-aab9-a21f81a5ab36",
                                DisplayOrder=1,
                                Visibility=true,
                                DisplayPrice = "0",
                                Price = 0,
                                PicturePath="https://cdn2.iconfinder.com/data/icons/location-map-vehicles/100/Locations-53-512.png",
                                Description = "açıklama bla bla bla",
                                SubCategoryLocalized = new List<Localized>
                                {
                                        new Localized
                                        {
                                             Language = Languages.Turkish,
                                             Text = "Stadyum"
                                        },
                                        new Localized
                                        {
                                             Language = Languages.English,
                                             Text = "Stadium"
                                        },
                                }
                            },
                            new SubCategory// takip etmediği bir kategori
                            {
                                Id = "24461fb6-323d-43e6-9a85-b263cff51bcc",
                                DisplayOrder=1,
                                Visibility=true,
                                DisplayPrice="1k",
                                PicturePath="http://chittagongit.com/images/team-icon-png/team-icon-png-20.jpg",
                                Description = "açıklama bla bla bla",
                                Price=1000,
                                SubCategoryLocalized = new List<Localized>
                                {
                                    new Localized
                                    {
                                         Language = Languages.Turkish,
                                         Text = "Takımlar"
                                    },
                                    new Localized
                                    {
                                         Language = Languages.English,
                                         Text = "Teams"
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
                                SubCategoryLocalized = new List<Localized>
                                {
                                    new Localized
                                    {
                                         Language = Languages.Turkish,
                                         Text = "Antrenörler"
                                    },
                                    new Localized
                                    {
                                         Language = Languages.English,
                                         Text = "Coaches"
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
                                SubCategoryLocalized = new List<Localized>
                                {
                                    new Localized
                                    {
                                         Language = Languages.Turkish,
                                         Text = "Hakkılı test"
                                    },
                                    new Localized
                                    {
                                         Language = Languages.English,
                                         Text = "Reference Test"
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
                    ISearchRepository searchRepository = Service.GetRequiredService<ISearchRepository>();
                    foreach (var category in subCategories)// yeni kaydolan kategorileri elasticsearch tarafına yolladık
                    {
                        foreach (var subCategory in category.SubCategories)
                        {
                            var @event = new NewSubCategoryAddedIntegrationEvent(subCategory.DisplayPrice,
                                                                            subCategory.PicturePath,
                                                                            subCategory.Price,
                                                                            subCategory.Id,
                                                                            category.Id,
                                                                            subCategory.SubCategoryLocalized.Select(s => new Localized
                                                                            {
                                                                                Language = s.Language,
                                                                                Text = s.Text
                                                                            }).ToList(),
                                                                            category.CategoryLocalized.Select(s => new Localized
                                                                            {
                                                                                Language = s.Language,
                                                                                Text = s.Text
                                                                            }).ToList());

                            ILogger<NewSubCategoryAddedIntegrationEventHandler> log = Service.GetRequiredService<ILogger<NewSubCategoryAddedIntegrationEventHandler>>();
                            await new NewSubCategoryAddedIntegrationEventHandler(searchRepository, log).Handle(@event);
                        }
                    }

                    LoadUsers(searchRepository);
                }
            });
        }

        private void LoadUsers(ISearchRepository searchRepository)
        {
            searchRepository.Insert(new Search
            {
                SearchType = SearchTypes.Player,
                FullName = "Ali Aldemir",
                UserId = "1111-1111-1111-1111",
                Language = null,// index için null atadık
                UserName = "witcherfearless",
                PicturePath = "http://i.pravatar.cc/150?u=witcherfearless",
                Id = "1111-1111-1111-1111",
                Suggest = new Nest.CompletionField
                {
                    Input = new string[] { "witcherfearless", "Ali Aldemir" }
                },
            });

            searchRepository.Insert(new Search
            {
                SearchType = SearchTypes.Player,
                FullName = "Demo",
                UserId = "2222-2222-2222-2222",
                Language = null,// index için null atadık
                UserName = "demo",
                PicturePath = "http://i.pravatar.cc/150?u=demo",
                Id = "2222-2222-2222-2222",
                Suggest = new Nest.CompletionField
                {
                    Input = new string[] { "demo", "Demo" }
                },
            });

            searchRepository.Insert(new Search
            {
                SearchType = SearchTypes.Player,
                FullName = "Bot",
                UserId = "3333-3333-3333-bot",
                Language = null,// index için null atadık
                UserName = "bot12345",
                PicturePath = "http://i.pravatar.cc/150?u=bot",
                Id = "3333-3333-3333-bot",
                Suggest = new Nest.CompletionField
                {
                    Input = new string[] { "bot12345", "Bot" }
                },
            });
        }
    }
}
