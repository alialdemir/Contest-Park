﻿using ContestPark.Core.CosmosDb.Models;
using ContestPark.Core.FunctionalTests;
using ContestPark.Post.API.Models;
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
                    .PostAsync(Entpoints.PostLike("410b33a7-cd16-4dc3-81ce-eb740fec9b78"), null);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact, TestPriority(1)]
        public async Task Get_post_likes_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetPostLikes("aee6685b-059e-4afe-b315-91146415e4b4"));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Theory, TestPriority(2)]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 1)]
        public async Task Get_paging_following_and_response_ok_status_code(int pageSize, int pageNumber)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetPostLikes("aee6685b-059e-4afe-b315-91146415e4b4", true, pageSize, pageNumber));

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
                    .GetAsync(Entpoints.GetPostLikes("aee6685b-059e-4afe-b315-91146415e4b4", true, 10, 2));

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
                    .PostAsync(Entpoints.PostLike("aee6685b-059e-4afe-b315-91146415e4b4"), null);

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
                    .DeleteAsync(Entpoints.DeleteUnLike("410b33a7-cd16-4dc3-81ce-eb740fec9b78"));

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
                    .DeleteAsync(Entpoints.DeleteUnLike("410b33a7-cd16-4dc3-81ce-eb740fec9b78"));

                string responseContent = await response.Content.ReadAsStringAsync();

                ValidationMessage message = JsonConvert.DeserializeObject<ValidationMessage>(responseContent);

                Assert.NotNull(message);
                Assert.Equal(errorMessage, message.ErrorMessage);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }
    }
}
