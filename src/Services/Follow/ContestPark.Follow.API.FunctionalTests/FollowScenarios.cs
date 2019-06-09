using ContestPark.Core.CosmosDb.Models;
using ContestPark.Follow.API.Models;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace ContestPark.Follow.API.FunctionalTests
{
    public class FollowScenarios : FollowScenariosBase
    {
        [Fact, TestPriority(1)]
        public async Task Get_get_all_followers_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetFollowers("1111-1111-1111-1111"));

                string responseContent = await response.Content.ReadAsStringAsync();

                ServiceModel<FollowModel> followers = JsonConvert.DeserializeObject<ServiceModel<FollowModel>>(responseContent);

                Assert.Single(followers.Items);

                FollowModel firstItem = followers.Items.First();

                Assert.Equal("Demo", firstItem.FullName);
                Assert.True(firstItem.IsFollowing);
                Assert.Equal("2222-2222-2222-2222", firstItem.UserId);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact, TestPriority(2)]
        public async Task Get_get_all_following_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetFollowing("1111-1111-1111-1111"));

                string responseContent = await response.Content.ReadAsStringAsync();

                ServiceModel<FollowModel> following = JsonConvert.DeserializeObject<ServiceModel<FollowModel>>(responseContent);

                Assert.Single(following.Items);

                FollowModel firstItem = following.Items.First();

                Assert.Equal("Demo", firstItem.FullName);
                Assert.True(firstItem.IsFollowing);
                Assert.Equal("2222-2222-2222-2222", firstItem.UserId);

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
                    .GetAsync(Entpoints.GetFollowing("2222-2222-2222-2222", true, pageSize, pageNumber));

                string responseContent = await response.Content.ReadAsStringAsync();

                ServiceModel<FollowModel> following = JsonConvert.DeserializeObject<ServiceModel<FollowModel>>(responseContent);

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

        [Theory, TestPriority(2)]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 1)]
        public async Task Get_paging_followers_and_response_ok_status_code(int pageSize, int pageNumber)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetFollowers("2222-2222-2222-2222", true, pageSize, pageNumber));

                string responseContent = await response.Content.ReadAsStringAsync();

                ServiceModel<FollowModel> followers = JsonConvert.DeserializeObject<ServiceModel<FollowModel>>(responseContent);

                Assert.Equal(pageNumber, followers.PageNumber);
                Assert.Equal(pageSize, followers.PageSize);
                Assert.Equal(pageSize, followers.Items.Count());
                if (pageSize == 1 && pageNumber == 1)
                {
                    Assert.True(followers.HasNextPage);
                }
                else if (pageNumber == 2)
                {
                    Assert.False(followers.HasNextPage);
                }
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact, TestPriority(9)]
        public async Task Delete_unfollow_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .DeleteAsync(Entpoints.Delete("2222-2222-2222-2222"));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Theory, TestPriority(6)]
        [InlineData("en-US", "You are not following this user.")]
        [InlineData("tr-TR", "Bu kullanıcıyı takip etmiyorsunuz.")]
        public async Task Delete_follow_and_response_badrequest_status_code_and_error_message(string langCode, string assetMessage)
        {
            using (var server = CreateServer())
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, Entpoints.Post("fake-userid"));

                var httpClient = server.CreateClient();

                httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(langCode));

                var response = await httpClient.SendAsync(httpRequestMessage);

                string responseContent = await response.Content.ReadAsStringAsync();

                ValidationMessage message = JsonConvert.DeserializeObject<ValidationMessage>(responseContent);

                Assert.NotNull(message);
                Assert.Equal(assetMessage, message.ErrorMessage);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact, TestPriority(3)]
        public async Task Post_follow_himself_and_response_badrequest_status_code()
        {
            using (var server = CreateServer())
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, Entpoints.Post("1111-1111-1111-1111"));

                var response = await server.CreateClient()
                    .SendAsync(httpRequestMessage);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact, TestPriority(4)]
        public async Task Post_follow_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, Entpoints.Post("3333-3333-3333-bot"));

                var response = await server.CreateClient()
                    .SendAsync(httpRequestMessage);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Theory, TestPriority(7)]
        [InlineData("en-US", "You are already following this user.")]
        [InlineData("tr-TR", "Bu kullanıcıyı zaten takip ediyorsunuz.")]
        public async Task Post_follow_and_response_badrequest_status_code_and_error_message(string langCode, string assetMessage)
        {
            using (var server = CreateServer())
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, Entpoints.Post("2222-2222-2222-2222"));

                var httpClient = server.CreateClient();

                httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(langCode));

                var response = await httpClient.SendAsync(httpRequestMessage);

                string responseContent = await response.Content.ReadAsStringAsync();

                ValidationMessage message = JsonConvert.DeserializeObject<ValidationMessage>(responseContent);

                Assert.NotNull(message);
                Assert.Equal(assetMessage, message.ErrorMessage);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }
    }

    internal class ValidationMessage
    {
        public string ErrorMessage { get; set; }
    }
}