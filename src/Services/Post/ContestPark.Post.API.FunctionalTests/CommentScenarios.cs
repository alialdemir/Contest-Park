using ContestPark.Core.FunctionalTests;
using ContestPark.Post.API.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ContestPark.Post.API.FunctionalTests
{
    public class CommentScenarios : PostScenariosBase
    {
        [Fact]
        public async Task Post_add_comment_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                CommentTextModel comment = new CommentTextModel
                {
                    Comment = "test comment"
                };

                string jsonContent = await Task.Run(() => JsonConvert.SerializeObject(comment));
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await server.CreateClient()
                    .PostAsync(Entpoints.PostComment(2), content);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Post_add_comment_and_response_badrequest_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .PostAsync(Entpoints.PostComment(1), null);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Theory]
        [InlineData("tr-TR", "Yorum ekleme işlemi başarısız oldu.")]
        [InlineData("en-US", "Adding comment failed.")]
        public async Task Post_add_comment_and_check_error_message(string langCode, string errorMessage)
        {
            using (var server = CreateServer())
            {
                CommentTextModel comment = new CommentTextModel
                {
                    Comment = "test comment"
                };

                string jsonContent = await Task.Run(() => JsonConvert.SerializeObject(comment));
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await server.CreateClient()
                    .AddLangCode(langCode)
                    .PostAsync(Entpoints.PostComment(9999), content);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                Assert.Equal(errorMessage, GetErrorMessage(response));
            }
        }
    }
}
