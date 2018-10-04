using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.Post;
using ContestPark.Mobile.Models.ServiceModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Post
{
    public class PostMockService : IPostService
    {
        public async Task<bool> DisLike(int postId)
        {
            await Task.Delay(300);
            return true;
        }

        public async Task<bool> Like(int postId)
        {
            await Task.Delay(300);
            return true;
        }

        public Task<ServiceModel<PostModel>> SubCategoryPostsAsync(short subCategoryId, PagingModel pagingModel)
        {
            return Task.FromResult(new ServiceModel<PostModel>()
            {
                Items = new List<PostModel>
                 {
                     new PostModel(),
                     new PostModel(),
                     new PostModel(),
                     new PostModel(),
                     new PostModel(),
                     new PostModel(),
                     new PostModel(),
                     new PostModel(),
                     new PostModel(),
                     new PostModel(),
                 }
            });
        }
    }
}