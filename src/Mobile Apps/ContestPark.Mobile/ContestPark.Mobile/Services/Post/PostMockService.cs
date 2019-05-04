using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.Post;
using ContestPark.Mobile.Models.ServiceModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Post
{
    public class PostMockService : IPostService
    {
        public async Task<bool> DisLikeAsync(string postId)
        {
            await Task.Delay(300);
            return true;
        }

        public async Task<bool> LikeAsync(string postId)
        {
            await Task.Delay(300);
            return true;
        }

        public Task<ServiceModel<PostModel>> GetPostsBySubCategoryIdAsync(short subCategoryId, PagingModel pagingModel)
        {
            List<PostModel> posts = new List<PostModel>();

            for (byte i = 0; i < 10; i++)
            {
                posts.Add(new PostModel
                {
                    CommentCount = i,
                    IsLike = false,
                    Date = DateTime.Now.AddDays(-i),
                    LikeCount = i,
                    PostId = Guid.NewGuid().ToString(),
                    PostType = Enums.PostTypes.Contest,
                    Bet = i * 123,

                    SubCategoryId = 1,
                    SubCategoryName = "Football Players",
                    SubCategoryPicturePath = DefaultImages.DefaultLock,

                    CompetitorFullName = "Elif Öz",
                    CompetitorProfilePicturePath = DefaultImages.DefaultProfilePicture,
                    CompetitorUserName = "elfoz",
                    CompetitorTrueAnswerCount = (byte)(i * 10),

                    FounderFullName = "Ali Aldemir",
                    FounderProfilePicturePath = DefaultImages.DefaultProfilePicture,
                    FounderTrueAnswerCount = (byte)(i * 7),
                    FounderUserName = "witcherfearless",
                });
            }

            return Task.FromResult(new ServiceModel<PostModel>
            {
                Count = 2,
                PageNumber = 1,
                PageSize = 1,
                Items = posts
            });
        }
    }
}