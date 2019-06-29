using ContestPark.Core.FunctionalTests;
using Newtonsoft.Json;
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
    }
}
