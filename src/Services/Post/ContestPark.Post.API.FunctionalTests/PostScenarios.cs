using ContestPark.Core.Database.Models;
using ContestPark.Core.FunctionalTests;
using ContestPark.Post.API.Enums;
using ContestPark.Post.API.Models;
using ContestPark.Post.API.Models.Post;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ContestPark.Post.API.FunctionalTests
{
    public class PostScenarios : PostScenariosBase
    {
        [Fact, TestPriority(1)]
        public async Task Post_like_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .PostAsync(Entpoints.PostLike(2), null);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact, TestPriority(1)]
        public async Task Get_post_detail_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetPostDetail(1));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact, TestPriority(1)]
        public async Task Get_post_detail_and_response_badrequest_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetPostDetail(0));

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact, TestPriority(1)]
        public async Task Get_subcategory_posts_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetPostsBySubcategoryId(1));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact, TestPriority(1)]
        public async Task Get_user_posts_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetPostsByUserIdAsync("witcherfearless"));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact, TestPriority(1)]
        public async Task Get_user_posts_and_response_notfound_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetPostsByUserIdAsync("fake-username"));

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact, TestPriority(1)]
        public async Task Get_bot_user_posts_and_response_notfound_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetPostsByUserIdAsync("bot12345"));

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact, TestPriority(1)]
        public async Task Get_user_posts_and_username_empty_and_response_badrequest_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetPostsByUserIdAsync(""));

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Theory, TestPriority(2)]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 1)]
        public async Task Get_subcategory_posts_and_check_paging(int pageSize, int pageNumber)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetPostsBySubcategoryId(1, true, pageSize, pageNumber));

                string responseContent = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                ServiceModel<PostModel> posts = JsonConvert.DeserializeObject<ServiceModel<PostModel>>(responseContent);

                Assert.Equal(pageNumber, posts.PageNumber);
                Assert.Equal(pageSize, posts.PageSize);
                Assert.Equal(pageSize, posts.Items.Count());
                if (pageSize == 1 && pageNumber == 1)
                {
                    Assert.True(posts.HasNextPage);
                }
                else if (pageNumber == 2)
                {
                    Assert.False(posts.HasNextPage);
                }
            }
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 1)]
        public async Task Get_post_detail_and_check_paging(int pageSize, int pageNumber)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetPostDetail(1, true, pageSize, pageNumber));

                string responseContent = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                PostDetailModel postDetail = JsonConvert.DeserializeObject<PostDetailModel>(responseContent);

                Assert.NotNull(postDetail.Post);

                Assert.Equal(pageNumber, postDetail.Comments.PageNumber);
                Assert.Equal(pageSize, postDetail.Comments.PageSize);
                Assert.Equal(pageSize, postDetail.Comments.Items.Count());
                if (pageSize == 1 && pageNumber == 1)
                {
                    Assert.True(postDetail.Comments.HasNextPage);
                }
                else if (pageNumber == 2)
                {
                    Assert.False(postDetail.Comments.HasNextPage);
                }
            }
        }

        [Fact, TestPriority(1)]
        public async Task Get_post_detail_and_check_data()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetPostDetail(1, true, 1, 1));

                string responseContent = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                PostDetailModel postDetail = JsonConvert.DeserializeObject<PostDetailModel>(responseContent);

                Assert.NotNull(postDetail.Post);

                var firstComment = postDetail.Comments.Items.FirstOrDefault();

                Assert.Equal("deneme", firstComment.Comment);
                Assert.Equal("1111-1111-1111-1111", firstComment.UserId);
                Assert.Equal("Ali Aldemir", firstComment.FullName);
                Assert.Equal("http://i.pravatar.cc/150?u=witcherfearless", firstComment.ProfilePicturePath);

                Assert.Equal("http://i.pravatar.cc/150?u=witcherfearless", postDetail.Post.PicturePath);
                Assert.Equal(4, postDetail.Post.CommentCount);
                Assert.Equal(7, postDetail.Post.LikeCount);
                Assert.Equal(1, postDetail.Post.PostId);
                Assert.Equal(PostTypes.Image, postDetail.Post.PostType);
                Assert.Equal("1111-1111-1111-1111", postDetail.Post.OwnerUserId);
                Assert.True(postDetail.Post.IsLike);
            }
        }

        [Theory, TestPriority(2)]
        [InlineData(1, 1)]
        [InlineData(1, 5)]
        [InlineData(2, 1)]
        public async Task Get_user_posts_and_check_paging(int pageSize, int pageNumber)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetPostsByUserIdAsync("witcherfearless", true, pageSize, pageNumber));

                string responseContent = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                ServiceModel<PostModel> posts = JsonConvert.DeserializeObject<ServiceModel<PostModel>>(responseContent);

                Assert.Equal(pageNumber, posts.PageNumber);
                Assert.Equal(pageSize, posts.PageSize);
                Assert.Equal(pageSize, posts.Items.Count());
                if (pageSize == 1 && pageNumber == 1)
                {
                    Assert.True(posts.HasNextPage);
                }
                else if (pageNumber == 5)
                {
                    Assert.False(posts.HasNextPage);
                }
            }
        }

        [Fact, TestPriority(1)]
        public async Task Get_post_likes_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetPostLikes(1));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Theory, TestPriority(1)]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 1)]
        public async Task Get_paging_following_and_check_paging(int pageSize, int pageNumber)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetPostLikes(1, true, pageSize, pageNumber));

                string responseContent = await response.Content.ReadAsStringAsync();

                ServiceModel<PostLikeModel> following = JsonConvert.DeserializeObject<ServiceModel<PostLikeModel>>(responseContent);

                Assert.Equal(pageNumber, following.PageNumber);
                Assert.Equal(pageSize, following.PageSize);
                Assert.Equal(pageSize, following.Items.Count());
                if (pageSize == 1 && pageNumber == 1)
                {
                    Assert.True(following.HasNextPage);
                }
                else if (pageNumber == 2)
                {
                    Assert.False(following.HasNextPage);
                }

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact, TestPriority(2)]
        public async Task Get_paging_following_and_response_notfound_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetPostLikes(1, true, 10, 2));

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Theory, TestPriority(1)]
        [InlineData("en-US", "You already liked this post.")]
        [InlineData("tr-TR", "Bu postu zaten beğenmişsiniz.")]
        [InlineData("fakelangoagecode", "You already liked this post.")]
        public async Task Post_like_and_response_notfound_status_code_and_check_errormessage(string langCode, string errorMessage)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .AddLangCode(langCode)
                    .PostAsync(Entpoints.PostLike(1), null);

                string responseContent = await response.Content.ReadAsStringAsync();

                ValidationMessage message = JsonConvert.DeserializeObject<ValidationMessage>(responseContent);

                Assert.NotNull(message);
                Assert.Equal(errorMessage, message.ErrorMessage);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact, TestPriority(2)]
        public async Task Delete_unlike_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .DeleteAsync(Entpoints.DeleteUnLike(1));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Theory, TestPriority(1)]
        [InlineData("en-US", "You have to like this post before to remove liking.")]
        [InlineData("tr-TR", "Beğeniyi kaldırmak için önce bu postu beğenmelisin.")]
        [InlineData("fakelangoagecode", "You have to like this post before to remove liking.")]
        public async Task Delete_unlike_and_response_notfound_status_code_and_check_errormessage(string langCode, string errorMessage)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .AddLangCode(langCode)
                    .DeleteAsync(Entpoints.DeleteUnLike(2));

                string responseContent = await response.Content.ReadAsStringAsync();

                ValidationMessage message = JsonConvert.DeserializeObject<ValidationMessage>(responseContent);

                Assert.NotNull(message);
                Assert.Equal(errorMessage, message.ErrorMessage);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }
    }
}
