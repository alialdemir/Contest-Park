using ContestPark.Core.DataSeed;
using ContestPark.Core.Enums;
using ContestPark.Core.Interfaces;
using ContestPark.Infrastructure.Category.Entities;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Infrastructure.Category.DataSeed
{
    public class CategoryContextSeed : ContextSeedBase
    {
        public override async Task SeedAsync(ISettingsBase settings, ILogger logger)
        {
            var policy = CreatePolicy();

            await policy.ExecuteAsync(async () =>
            {
                ConnectionString = settings.ConnectionString;

                Logger = logger;

                SeedName = nameof(CategoryContextSeed);

                await InsertDataAsync(GetCategories());

                await InsertDataAsync(GetCategoryLangs());

                await InsertDataAsync(GetSubCategories());

                await InsertDataAsync(GetSubCategoryLangs());

                await InsertDataAsync(GetOpenSubCategories());
            });
        }

        private IEnumerable<CategoryEntity> GetCategories()
        {
            return new List<CategoryEntity>
            {
                new CategoryEntity
                {
                   Order=0,
                   Visibility=true,
                   Color ="#F7931B",
                }
            };
        }

        private IEnumerable<CategoryLangEntity> GetCategoryLangs()
        {
            return new List<CategoryLangEntity>
                     {
                         new CategoryLangEntity
                         {
                             CategoryId=1,
                             CategoryName = "Bayraklar",
                              LanguageId = Languages.Turkish,
                         },
                         new CategoryLangEntity
                         {
                             CategoryId= 1,
                             CategoryName = "Flags",
                             LanguageId = Languages.English,
                         }
                    };
        }

        private IEnumerable<OpenSubCategoryEntity> GetOpenSubCategories()
        {
            string userId = "1111-1111-1111-1111";
            string demoUserId = "2222-2222-2222-2222";

            return new List<OpenSubCategoryEntity>
            {
                new OpenSubCategoryEntity
                {
                     UserId = userId,
                     SubCategoryId = 1,
                },
                //new OpenSubCategoryEntity
                //{
                //     UserId = userId,
                //      SubCategoryId =  2,
                //},
                //new OpenSubCategoryEntity
                //{
                //     UserId = userId,
                //      SubCategoryId = 3,
                //},
                //new OpenSubCategoryEntity
                //{
                //     UserId = demoUserId,
                //      SubCategoryId = 1,
                //}
            };
        }

        private IEnumerable<SubCategoryEntity> GetSubCategories()
        {
            return new List<SubCategoryEntity>
            {
                new SubCategoryEntity
                {
                    CategoryId=1,
                    Order=0,
                    Visibility=true,
                    DisplayPrice="100k",
                    PictuePath="https://5.imimg.com/data5/IJ/JK/MY-11744895/playing-football-500x500.jpg",
                    Price=100000,
                },
                //new SubCategoryEntity
                //{
                //    CategoryId=1,
                //    Order=0,
                //    Visibility=true,
                //    DisplayPrice="120k",
                //    PictuePath="https://5.imimg.com/data5/IJ/JK/MY-11744895/playing-football-500x500.jpg",
                //    Price=120000,
                //},
                //new SubCategoryEntity
                //{
                //    CategoryId=1,
                //    Order=0,
                //    Visibility=true,
                //    DisplayPrice="1k",
                //    PictuePath="#",
                //    Price=1000,
                //},
                //new SubCategoryEntity
                //{
                //    CategoryId=1,
                //    Order=0,
                //    Visibility=true,
                //    DisplayPrice="150k",
                //    PictuePath="#",
                //    Price=150000,
                //}
            };
        }

        private IEnumerable<SubCategoryLangEntity> GetSubCategoryLangs()
        {
            return new List<SubCategoryLangEntity>
            {
                new SubCategoryLangEntity
                {
                    SubCategoryId = 1,
                    LanguageId = Languages.Turkish,
                    SubCategoryName = "Bayraklar"
                },
                new SubCategoryLangEntity
                {
                    SubCategoryId = 1,
                    LanguageId = Languages.English,
                    SubCategoryName = "Flags"
                },
                        //new SubCategoryLangEntity
                        //{
                        //     SubCategoryId = 1,
                        //     LanguageId = Languages.Turkish,
                        //     SubCategoryName = "Hakem"
                        //},
                        //new SubCategoryLangEntity
                        //{
                        //     SubCategoryId = 1,
                        //     LanguageId = Languages.English,
                        //     SubCategoryName = "Referee"
                        //},

                        //new SubCategoryLangEntity
                        //{
                        //     SubCategoryId = 2,
                        //     LanguageId = Languages.Turkish,
                        //     SubCategoryName = "Stadyum"
                        //},
                        //new SubCategoryLangEntity
                        //{
                        //     SubCategoryId = 2,
                        //     LanguageId = Languages.English,
                        //     SubCategoryName = "Stadium"
                        //},

                        //new SubCategoryLangEntity
                        //{
                        //     SubCategoryId = 3,
                        //     LanguageId = Languages.Turkish,
                        //     SubCategoryName = "Takımlar"
                        //},
                        //new SubCategoryLangEntity
                        //{
                        //     SubCategoryId = 3,
                        //     LanguageId = Languages.English,
                        //     SubCategoryName = "Teams"
                        //},

                        //new SubCategoryLangEntity
                        //{
                        //     SubCategoryId = 4,
                        //     LanguageId = Languages.Turkish,
                        //     SubCategoryName = "Antrenörler"
                        //},
                        //new SubCategoryLangEntity
                        //{
                        //     SubCategoryId = 4,
                        //     LanguageId = Languages.English,
                        //     SubCategoryName = "Coaches"
                        //}
            };
        }
    }
}