using ContestPark.Chat.API.Model;
using ContestPark.Core.Database.Models;
using ContestPark.Core.FunctionalTests;
using Newtonsoft.Json;
using System.Linq;
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

        [Fact, TestPriority(1)]
        public async Task Get_user_messages_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetUserMessages());

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Get_user_messages_and_check_paging_values()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetUserMessages(true, 1, 1));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                string responseContent = await response.Content.ReadAsStringAsync();

                ServiceModel<MessageModel> result = JsonConvert.DeserializeObject<ServiceModel<MessageModel>>(responseContent);

                Assert.NotNull(result);
                Assert.Equal(1, result.PageNumber);
                Assert.Equal(1, result.PageSize);
                Assert.Single(result.Items);
                Assert.False(result.HasNextPage);
            }
        }

        [Fact, TestPriority(1)]
        public async Task Post_all_read_messages_and_unread_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .PostAsync(Entpoints.PostReadMessages(2), null);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact, TestPriority(1)]
        public async Task Get_conversation_detail_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetConversationDetail(1));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Theory, TestPriority(1)]
        [InlineData("tr-TR", "Bu konuşma size ait değil.")]
        [InlineData("en-US", "This conversation is not yours.")]
        public async Task Get_conversation_detail_and_check_response_message(string langCode, string errorMessage)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .AddLangCode(langCode)
                    .GetAsync(Entpoints.GetConversationDetail(9999));

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                Assert.Equal(errorMessage, GetErrorMessage(response));
            }
        }

        [Fact]
        public async Task Get_conversation_detail_and_check_paging_values()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                  .GetAsync(Entpoints.GetConversationDetail(1, true, 1, 1));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                string responseContent = await response.Content.ReadAsStringAsync();

                ServiceModel<ConversationDetailModel> result = JsonConvert.DeserializeObject<ServiceModel<ConversationDetailModel>>(responseContent);

                Assert.NotNull(result);
                Assert.Equal(1, result.PageNumber);
                Assert.Equal(1, result.PageSize);
                Assert.Single(result.Items);
                Assert.False(result.HasNextPage);
            }
        }

        [Fact, TestPriority(2)]
        public async Task Delete_messages_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .DeleteAsync(Entpoints.DeleteMessages(1));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Theory, TestPriority(1)]
        [InlineData("tr-TR", "Bu konuşma size ait değil.")]
        [InlineData("en-US", "This conversation is not yours.")]
        public async Task Delete_messages_and_conversation_is_not_yours_and_response_badrequest_status_code(string langCode, string message)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .AddLangCode(langCode)
                    .DeleteAsync(Entpoints.DeleteMessages(2));

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                Assert.Equal(message, GetErrorMessage(response));
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

        [Fact, TestPriority(4)]
        public async Task Get_user_blocked_list_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                       .GetAsync(Entpoints.GetUserBlockedList());

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact, TestPriority(4)]
        public async Task Get_user_blocked_list_and_check_values()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                       .GetAsync(Entpoints.GetUserBlockedList());

                var jsonData = response.Content.ReadAsStringAsync().Result;

                ServiceModel<BlockModel> result = JsonConvert.DeserializeObject<ServiceModel<BlockModel>>(jsonData);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                Assert.NotNull(result.Items);

                BlockModel firstItem = result.Items.FirstOrDefault();

                Assert.Equal("3333-3333-3333-bot", firstItem.UserId);

                Assert.Equal("Bot", firstItem.FullName);

                Assert.True(firstItem.IsBlocked);
            }
        }

        [Fact]
        public async Task Get_user_blocked_list_and_check_paging_values()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetUserBlockedList(true, 1, 1));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                string responseContent = await response.Content.ReadAsStringAsync();

                ServiceModel<BlockModel> result = JsonConvert.DeserializeObject<ServiceModel<BlockModel>>(responseContent);

                Assert.NotNull(result);
                Assert.Equal(1, result.PageNumber);
                Assert.Equal(1, result.PageSize);
                Assert.Single(result.Items);
                Assert.False(result.HasNextPage);
            }
        }

        private class IsBlockedModel
        {
            public bool IsBlocked { get; set; }
        }

        private class UnReadMessage
        {
            public int UnReadMessageCount { get; set; }
        }
    }
}
