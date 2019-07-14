using ContestPark.Core.Database.Infrastructure;
using ContestPark.Post.API.Infrastructure.Tables;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Post.API.Infrastructure
{
    public class PostApiSeed : ContextSeedBase<PostApiSeed>
    {
        public async Task SeedAsync(IServiceProvider service, ILogger<PostApiSeed> logger)
        {
            var policy = CreatePolicy();

            Service = service;
            Logger = logger;

            await policy.ExecuteAsync(async () =>
            {
                await InsertDataAsync(new List<Tables.Post.Post>
                {
                        new Tables.Post.Post
                    {
                            PostType = Enums.PostTypes.Image,
                            PostImageType = Enums.PostImageTypes.ProfileImage,
                            OwnerUserId = "1111-1111-1111-1111",
                            PicturePath= "http://i.pravatar.cc/150?u=witcherfearless"
                    },
                        new Tables.Post.Post
                    {
                            PostType = Enums.PostTypes.Image,
                            PostImageType = Enums.PostImageTypes.CoverImage,
                            OwnerUserId = "1111-1111-1111-1111",
                            PicturePath= "http://i.pravatar.cc/150?u=witcherfearlessCOVER"
                    },
                        new Tables.Post.Post
                    {
                            PostType = Enums.PostTypes.Text,
                            OwnerUserId = "1111-1111-1111-1111",
                            Description = "İlk postumu yazdım oleyyy",
                            LikeCount = 2,
                            CommentCount = 1
                    },
                        new Tables.Post.Post
                    {
                            PostType = Enums.PostTypes.Contest,
                            OwnerUserId = "1111-1111-1111-1111",
                            FounderUserId = "1111-1111-1111-1111",
                            FounderTrueAnswerCount = 253,
                            Bet = 500,
                            DuelId = 1,
                            SubCategoryId = 1,
                           CompetitorTrueAnswerCount = 35,
                           CompetitorUserId = "2222-2222-2222-2222",
                    },
                        new Tables.Post.Post
                    {
                            PostType = Enums.PostTypes.Contest,
                            OwnerUserId = "1111-1111-1111-1111",
                            FounderUserId = "1111-1111-1111-1111",
                            FounderTrueAnswerCount = 23,
                            Bet = 10000,
                            DuelId = 1,
                            SubCategoryId = 1,
                           CompetitorTrueAnswerCount = 44,
                           CompetitorUserId = "2222-2222-2222-2222",
                    },
                });

                await InsertDataAsync(new List<Like>
                {
                    new Like
                    {
                            UserId = "1111-1111-1111-1111",
                            PostId = 1
                    },
                    new Like
                    {
                            UserId = "2222-2222-2222-2222",
                            PostId = 1
                    },
                });
            });
        }
    }
}
