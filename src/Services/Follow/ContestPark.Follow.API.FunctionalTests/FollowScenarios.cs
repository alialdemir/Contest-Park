using ContestPark.Core.Enums;
using ContestPark.Follow.API.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

                List<FollowModel> followers = JsonConvert.DeserializeObject<List<FollowModel>>(responseContent);

                Assert.Single(followers);

                Assert.Equal("Demo", followers.First().FullName);
                Assert.True(followers.First().IsFollowing);
                Assert.Equal("2222-2222-2222-2222", followers.First().UserId);

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

                List<FollowModel> followers = JsonConvert.DeserializeObject<List<FollowModel>>(responseContent);

                Assert.Single(followers);

                Assert.Equal("Demo", followers.First().FullName);
                Assert.True(followers.First().IsFollowing);
                Assert.Equal("2222-2222-2222-2222", followers.First().UserId);

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