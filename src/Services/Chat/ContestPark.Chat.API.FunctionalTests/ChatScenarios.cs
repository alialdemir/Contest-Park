using ContestPark.Chat.API.Model;
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
        [Fact]
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
    }
}