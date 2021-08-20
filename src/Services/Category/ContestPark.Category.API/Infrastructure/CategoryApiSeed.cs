using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ContestPark.Category.API.Extensions;
using ContestPark.Category.API.Infrastructure.Repositories.Search;
using ContestPark.Category.API.Infrastructure.Tables;
using ContestPark.Core.Database.Infrastructure;
using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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

                IRepository<Artist> artistRepository = (IRepository<Artist>)Service.GetRequiredService(typeof(IRepository<Artist>));
                IRepository<Song> songRepository = (IRepository<Song>)Service.GetRequiredService(typeof(IRepository<Song>));
                IRepository<Lyrics> lyricsRepository = (IRepository<Lyrics>)Service.GetRequiredService(typeof(IRepository<Lyrics>));
                IRepository<Tables.Category> categoryRepository = (IRepository<Tables.Category>)Service.GetRequiredService(typeof(IRepository<Tables.Category>));
                IRepository<SubCategory> subCategorysRepository = (IRepository<SubCategory>)Service.GetRequiredService(typeof(IRepository<SubCategory>));

                if (await artistRepository.CountAsync() != 0)
                {
                    return;
                }

                List<Tables.Artist> artists = GetArtists();

                foreach (var artist in artists)
                {
                    string[] artistSplit = artist.ArtistName.Split('&', System.StringSplitOptions.None);
                    foreach (var artistName in artistSplit)
                    {
                        int isArtistExits = artistRepository.QuerySingleOrDefault<int>($"select count(*) from Artists where ArtistName=@ArtistName", new
                        {
                            ArtistName = artistName.Trim()
                        });
                        if (isArtistExits > 0)
                            continue;

                        int? categoryId = await categoryRepository.AddAsync(new Tables.Category
                        {
                            DisplayOrder = 0,
                            Visibility = true,
                        });

                        int? artistId = await artistRepository.AddAsync(new Tables.Artist
                        {
                            ArtistName = artistName.Trim(),
                            CoverPicturePath = artist.CoverPicturePath,
                            Description = artist.Description,
                            PicturePath = artist.PicturePath ?? "#",
                            Slug = artistName.GenerateSlug(),
                            CategoryId = Convert.ToInt16(categoryId.Value),
                        });


                        foreach (var song in artist.Songs)
                        {
                            int? subCategoryId = await subCategorysRepository.AddAsync(new SubCategory
                            {
                                DisplayOrder = 2,
                                Visibility = true,
                                DisplayPrice = "0",
                                PicturePath = song.PicturePath,
                                Price = 0,
                                FollowerCount = 1,
                            });

                            await InsertDataAsync(new List<SubCategoryLocalized>
                            {
                                new SubCategoryLocalized
                                {
                                    Language = Languages.Turkish,
                                    SubCategoryName = "Antrenörler",
                                    Description = "Antrenörler description",
                                    SubCategoryId = Convert.ToInt16(subCategoryId.Value)
                                }
                            });

                            song.ArtistId = Convert.ToInt16(artistId.Value);
                            song.SubCategoryId = Convert.ToInt16(subCategoryId.Value);

                            int? songId = await songRepository.AddAsync(song);

                            foreach (var lyrics in song.Lyricss)
                            {
                                lyrics.SongId = Convert.ToInt16(songId.Value);

                                await lyricsRepository.AddAsync(lyrics);
                            }
                        }
                    }
                }

                //var subCategories = new List<SubCategory>
                //{
                //                new SubCategory
                //            {
                //                DisplayOrder = 2,
                //                Visibility = true,
                //                DisplayPrice = "0",
                //                PicturePath = "https://static.thenounproject.com/png/14039-200.png",
                //                Price = 0,
                //                FollowerCount = 1,
                //            },

                //            new SubCategory
                //            {
                //                DisplayOrder=1,
                //                Visibility=true,
                //                DisplayPrice = "0",
                //                Price = 0,
                //                PicturePath="https://cdn2.iconfinder.com/data/icons/location-map-vehicles/100/Locations-53-512.png",
                //            },
                //            new SubCategory// takip etmediği bir kategori
                //            {
                //                DisplayOrder=1,
                //                Visibility=true,
                //                DisplayPrice="1k",
                //                PicturePath="http://chittagongit.com/images/team-icon-png/team-icon-png-20.jpg",
                //                Price = 1.00m,
                //            },
                //            new SubCategory
                //            {
                //                DisplayOrder=3,
                //                Visibility=true,
                //                DisplayPrice = "10k",
                //                PicturePath="https://d2n3notmdf08g1.cloudfront.net/common/Login/gotr_icon_whistle.png",
                //                Price = 10.00m,
                //                FollowerCount =1
                //            },
                //};

                //await InsertDataAsync(subCategories);

                //var subCategoryLocalized = new List<SubCategoryLocalized> {
                //    new SubCategoryLocalized
                //    {
                //         Language = Languages.Turkish,
                //         SubCategoryName = "Hakem",
                //         Description = "Hakem açıklama",
                //         SubCategoryId = 1
                //    },
                //    new SubCategoryLocalized
                //    {
                //         Language = Languages.English,
                //         SubCategoryName = "Referee",
                //         Description = "Referee description",
                //         SubCategoryId = 1
                //    },

                //    new SubCategoryLocalized
                //    {
                //         Language = Languages.Turkish,
                //         SubCategoryName = "Stadyum",
                //         Description = "Stadyum açıklama",
                //         SubCategoryId = 2
                //    },
                //    new SubCategoryLocalized
                //    {
                //         Language = Languages.English,
                //         SubCategoryName = "Stadium",
                //         Description = "Stadium description",
                //         SubCategoryId = 2
                //    },

                //    new SubCategoryLocalized
                //    {
                //         Language = Languages.Turkish,
                //         SubCategoryName = "Takımlar",
                //         Description = "Takımlar description",
                //         SubCategoryId = 3
                //    },
                //    new SubCategoryLocalized
                //    {
                //         Language = Languages.English,
                //         SubCategoryName = "Teams",
                //         Description = "Teams description",
                //         SubCategoryId = 3
                //    },

                //    new SubCategoryLocalized
                //    {
                //         Language = Languages.Turkish,
                //         SubCategoryName = "Antrenörler",
                //         Description = "Antrenörler description",
                //         SubCategoryId = 4
                //    },
                //    new SubCategoryLocalized
                //    {
                //         Language = Languages.English,
                //         SubCategoryName = "Coaches",
                //         Description = "Coaches description",
                //         SubCategoryId = 4
                //    },
                //};
                //await InsertDataAsync(subCategoryLocalized);

                //var categories = new List<Tables.Category>
                //{
                //    new Tables.Category
                //    {
                //        DisplayOrder = 0,
                //        Visibility =true,
                //    },
                //};

                //bool isAddedCategories = await InsertDataAsync<short, Tables.Category>(categories);

                //List<SubCategoryOfCategory> subcategoriesOfCategory = new List<SubCategoryOfCategory>();
                //List<CategoryLocalized> categoryLocalized = new List<CategoryLocalized>();

                //categoryLocalized = new List<CategoryLocalized>
                //    {
                //        new CategoryLocalized
                //        {
                //            Text = "Futbol",
                //            Language = Languages.Turkish,
                //            CategoryId =1,
                //        },
                //        new CategoryLocalized
                //        {
                //            Text = "Football",
                //            Language = Languages.English,
                //            CategoryId =1,
                //        },
                //    };

                //await InsertDataAsync(categoryLocalized);
                //// todo category subcategory ilişki

                //subcategoriesOfCategory = new List<SubCategoryOfCategory>
                //    {
                //        new SubCategoryOfCategory
                //        {
                //             CategoryId = 1,
                //             SubCategoryId = 1
                //        },
                //        new SubCategoryOfCategory
                //        {
                //             CategoryId = 1,
                //             SubCategoryId = 2
                //        },
                //        new SubCategoryOfCategory
                //        {
                //             CategoryId = 1,
                //             SubCategoryId = 3
                //        },
                //        new SubCategoryOfCategory
                //        {
                //             CategoryId = 1,
                //             SubCategoryId = 4
                //        },
                //    };

                //await InsertDataAsync(subcategoriesOfCategory);

                //await InsertDataAsync(new List<OpenSubCategory>
                //{
                //    new OpenSubCategory
                //    {
                //         SubCategoryId = 2,
                //         UserId = "1111-1111-1111-1111"
                //    },
                //    new OpenSubCategory
                //    {
                //         SubCategoryId = 4,
                //         UserId = "1111-1111-1111-1111"
                //    },
                //});

                //await InsertDataAsync(new List<FollowSubCategory>
                //{
                //    new FollowSubCategory
                //    {
                //        UserId = "1111-1111-1111-1111",
                //        SubCategoryId= 2,
                //    },
                //    new FollowSubCategory
                //    {
                //        UserId = "1111-1111-1111-1111",
                //        SubCategoryId= 4,
                //    }
                //});

                //if (isAddedCategories)
                //{
                //    ISearchRepository searchRepository = Service.GetRequiredService<ISearchRepository>();

                //    foreach (var categoryOf in subcategoriesOfCategory)
                //    {
                //        var subCategory = subCategories[categoryOf.SubCategoryId - 1];
                //        var category = categoryLocalized.Where(x => x.CategoryId == categoryOf.CategoryId).ToList();
                //        var subCategoryLocalized1 = subCategoryLocalized.Where(x => x.SubCategoryId == categoryOf.SubCategoryId).ToList();

                //        var @event = new NewSubCategoryAddedIntegrationEvent(subCategory.DisplayPrice,
                //                                                        subCategory.PicturePath,
                //                                                        subCategory.Price,
                //                                                        categoryOf.SubCategoryId,
                //                                                        categoryOf.CategoryId,
                //                                                        subCategoryLocalized1.Select(s => new Localized
                //                                                        {
                //                                                            Language = s.Language,
                //                                                            Text = s.SubCategoryName
                //                                                        }).ToList(),
                //                                                        category.Select(s => new Localized
                //                                                        {
                //                                                            Language = s.Language,
                //                                                            Text = s.Text
                //                                                        }).ToList());

                //        ILogger<NewSubCategoryAddedIntegrationEventHandler> log = Service.GetRequiredService<ILogger<NewSubCategoryAddedIntegrationEventHandler>>();
                //        await new NewSubCategoryAddedIntegrationEventHandler(searchRepository, log).Handle(@event);
                //    }

                //    LoadUsers(searchRepository);
                //}
            });
        }


        private List<Tables.Artist> GetArtists()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "ContestPark.Category.API.SeedData.tr.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();

                var artists = JsonConvert.DeserializeObject<List<Tables.Artist>>(result);

                return artists;
            }
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