using ContestPark.Chat.API.Model;
using ContestPark.Core.FunctionalTests;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ContestPark.Chat.API.FunctionalTests
{
    public class ChatScenarios : ChatScenariosBase
    {
        [Fact, TestPriority(1)]
        public async Task Post_send_message_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                Message purchase = new Message
                {
                    ReceiverUserId = "2222-2222-2222-2222",
                    Text = "Test mesajı"
                };

                string jsonContent = await Task.Run(() => JsonConvert.SerializeObject(purchase));
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await server.CreateClient()
                    .PostAsync(Entpoints.Post(), content);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Theory, TestPriority(2)]
        [InlineData("en-US", "You can not send messages to yourself.")]
        [InlineData("tr-TR", "Kendinize mesaj gönderemezsiniz.")]
        public async Task Post_block_yourself_and_check_error_meesage(string langCode, string errorMessage)
        {
            using (var server = CreateServer())
            {
                Message purchase = new Message
                {
                    ReceiverUserId = "1111-1111-1111-1111",
                    Text = "Test mesajı"
                };

                string jsonContent = await Task.Run(() => JsonConvert.SerializeObject(purchase));
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await server.CreateClient()
                    .AddLangCode(langCode)
                    .PostAsync(Entpoints.Post(), content);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                Assert.Equal(errorMessage, GetErrorMessage(response));
            }
        }

        [Theory, TestPriority(2)]
        [InlineData("en-US", "This user is already blocked.")]
        [InlineData("tr-TR", "Bu kullanıcı zaten engellendi.")]
        public async Task Post_block_and_check_error_meesage(string langCode, string errorMessage)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .AddLangCode(langCode)
                    .PostAsync(Entpoints.PostBlock("3333-3333-3333-bot"), null);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                Assert.Equal(errorMessage, GetErrorMessage(response));
            }
        }

        [Fact, TestPriority(4)]
        public async Task Post_block_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                       .PostAsync(Entpoints.PostBlock("2222-2222-2222-2222"), null);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact, TestPriority(5)]
        public async Task Post_unblock_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                       .DeleteAsync(Entpoints.PostUnBlock("3333-3333-3333-bot"));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact, TestPriority(3)]
        public async Task Delete_unblock_and_check_error_meesage()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .DeleteAsync(Entpoints.PostUnBlock("2222-2222-2222-2222"));

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                Assert.Equal("You have not blocked this user.", GetErrorMessage(response));
            }
        }

        [Fact]
        public async Task Delete_unblock_yourself_and_response_badrequest()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .DeleteAsync(Entpoints.PostUnBlock("1111-1111-1111-1111"));

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact, TestPriority(0)]
        public async Task Get_blocked_status_and_response_ok_status_code_and_isBlocked_true()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetBlockedStatus("2222-2222-2222-2222"));

                var jsonData = response.Content.ReadAsStringAsync().Result;

                IsBlockedModel result = JsonConvert.DeserializeObject<IsBlockedModel>(jsonData);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                Assert.False(result.IsBlocked);
            }
        }

        [Fact, TestPriority(0)]
        public async Task Get_blocked_status_and_response_ok_status_code_and_isBlocked_false()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetBlockedStatus("3333-3333-3333-bot"));

                var jsonData = response.Content.ReadAsStringAsync().Result;

                IsBlockedModel result = JsonConvert.DeserializeObject<IsBlockedModel>(jsonData);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                Assert.True(result.IsBlocked);
            }
        }

        [Fact]
        public async Task Get_blocked_status_yourself_and_response_badrequest()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetBlockedStatus("1111-1111-1111-1111"));

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        private class IsBlockedModel
        {
            public bool IsBlocked { get; set; }
        }
    }
}