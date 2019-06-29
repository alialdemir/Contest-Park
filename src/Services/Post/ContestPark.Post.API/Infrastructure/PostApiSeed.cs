using ContestPark.Core.CosmosDb.Infrastructure;
using ContestPark.Post.API.Infrastructure.Documents;
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
                await InsertDataAsync(new List<Documents.Post>
                {
                        new Documents.Post
                    {
                            PostType = Enums.PostTypes.Image,
                            PostImageType = Enums.PostImageTypes.ProfileImage,
                            OwnerUserId = "1111-1111-1111-1111",
                            PicturePath= "http://i.pravatar.cc/150?u=witcherfearless"
                    },
                        new Documents.Post
                    {
                            PostType = Enums.PostTypes.Image,
                            PostImageType = Enums.PostImageTypes.CoverImage,
                            OwnerUserId = "1111-1111-1111-1111",
                            PicturePath= "http://i.pravatar.cc/150?u=witcherfearlessCOVER"
                    },
                        new Documents.Post
                    {
                            Id = "aee6685b-059e-4afe-b315-91146415e4b4",
                            PostType = Enums.PostTypes.Text,
                            OwnerUserId = "1111-1111-1111-1111",
                            Description = "İlk postumu yazdım oleyyy",
                            LikeCount = 2,
                            CommentCount = 1
                    },
                        new Documents.Post
                    {
                            Id = "410b33a7-cd16-4dc3-81ce-eb740fec9b78",
                            PostType = Enums.PostTypes.Contest,
                            OwnerUserId = "1111-1111-1111-1111",
                            FounderUserId = "1111-1111-1111-1111",
                            FounderTrueAnswerCount = 253,
                            Bet = 500,
                            DuelId = "123456789",// TODO: Bu id'yi duel servisinin eklediği bir düello ile eşleşmesi
                            SubCategoryId = "7c3a26b7-74df-4128-aab9-a21f81a5ab36",
                           CompetitorTrueAnswerCount = 35,
                           CompetitorUserId = "2222-2222-2222-2222",
                    },
                });

                await InsertDataAsync(new List<Like>
                {
                    new Like
                    {
                            UserId = "1111-1111-1111-1111",
                            PostId = "aee6685b-059e-4afe-b315-91146415e4b4"
                    },
                    new Like
                    {
                            UserId = "2222-2222-2222-2222",
                            PostId = "aee6685b-059e-4afe-b315-91146415e4b4"
                    },
                });
            });
        }
    }
}
