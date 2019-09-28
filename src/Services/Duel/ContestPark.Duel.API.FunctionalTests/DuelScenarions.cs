using ContestPark.Core.FunctionalTests;
using ContestPark.Duel.API.Enums;
using ContestPark.Duel.API.Models;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ContestPark.Duel.API.FunctionalTests
{
    public class DuelScenarions : DuelScenariosBase
    {
        [Theory]
        [InlineData(BalanceTypes.Gold)]
        [InlineData(BalanceTypes.Money)]
        public async Task Post_standbymode_and_response_ok_status_code(BalanceTypes balanceType)
        {
            using (var server = CreateServer())
            {
                StandbyModeModel standbyMode = new StandbyModeModel
                {
                    BalanceType = balanceType,
                    Bet = 1234,
                    ConnectionId = Guid.NewGuid().ToString(),
                    SubCategoryId = 1,
                };

                string jsonContent = await Task.Run(() => JsonConvert.SerializeObject(standbyMode));
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await server.CreateClient()
                    .PostAsync(Entpoints.PostAddStandbyMode(), content);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Get_duel_result_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                   .GetAsync(Entpoints.GetDuelResult(1));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Theory]
        [InlineData(BalanceTypes.Gold)]
        [InlineData(BalanceTypes.Money)]
        public async Task Get_add_opponent_and_response_ok_status_code(BalanceTypes balanceType)
        {
            using (var server = CreateServer())
            {
                StandbyModeModel standbyMode = new StandbyModeModel
                {
                    BalanceType = balanceType,
                    Bet = 1234,
                    ConnectionId = Guid.NewGuid().ToString(),
                    SubCategoryId = 1,
                };

                string jsonContent = await Task.Run(() => JsonConvert.SerializeObject(standbyMode));
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await server.CreateClient()
                    .PostAsync(Entpoints.PostAddOpponent(), content);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Get_duel_result_and_check_data()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                   .GetAsync(Entpoints.GetDuelResult(1));

                string responseContent = await response.Content.ReadAsStringAsync();

                DuelResultModel duelResult = JsonConvert.DeserializeObject<DuelResultModel>(responseContent);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("1111-1111-1111-1111", duelResult.FounderUserId);
                Assert.Equal("2222-2222-2222-2222", duelResult.OpponentUserId);

                Assert.Equal("Ali Aldemir", duelResult.FounderFullName);
                Assert.Equal("Demo", duelResult.OpponentFullName);

                Assert.Equal("witcherfearless", duelResult.FounderUserName);
                Assert.Equal("demo", duelResult.OpponentUserName);

                Assert.Equal("http://i.pravatar.cc/150?u=witcherfearless", duelResult.FounderProfilePicturePath);
                Assert.Equal("http://i.pravatar.cc/150?u=demo", duelResult.OpponentProfilePicturePath);

                Assert.Equal(255, duelResult.FounderScore);
                Assert.Equal(200, duelResult.OpponentScore);

                Assert.Equal(1, duelResult.SubCategoryId);
                Assert.Equal("Refree", duelResult.SubCategoryName);
                Assert.Equal("https://static.thenounproject.com/png/14039-200.png", duelResult.SubCategoryPicturePath);

                Assert.Equal((byte?)70, duelResult.VictoryBonus);
                Assert.Equal((byte?)40, duelResult.FinishBonus);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(4)]
        public async Task Get_duel_result_and_response_bad_request_status_code(int duelId)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                   .GetAsync(Entpoints.GetDuelResult(duelId));

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Theory]
        [InlineData("tr-TR", "Bitmiş düellodan çıkamazsınız.")]
        [InlineData("en-US", "You can't leave the finished duel.")]
        public async Task Post_duelescape_and_response_ok_status_code(string langCode, string errorMessage)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .AddLangCode(langCode)
                    .PostAsync(Entpoints.PostDuelEscape(1), null);

                string responseContent = await response.Content.ReadAsStringAsync();

                ValidationMessage message = JsonConvert.DeserializeObject<ValidationMessage>(responseContent);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                Assert.Equal(errorMessage, message.ErrorMessage);
            }
        }

        [Fact]
        public async Task Post_duelescape_and_response_bad_request_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .PostAsync(Entpoints.PostDuelEscape(1), null);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Theory]
        [InlineData(-1, 1, "abc")]
        [InlineData(1, -3456, "ro")]
        [InlineData(1, 1, "")]
        public async Task Post_standbymode_and_response_badRequest_status_code(decimal bet, short subCategoryId, string connectionId)
        {
            using (var server = CreateServer())
            {
                StandbyModeModel standbyMode = new StandbyModeModel
                {
                    BalanceType = BalanceTypes.Gold,
                    Bet = bet,
                    ConnectionId = connectionId,
                    SubCategoryId = subCategoryId,
                };

                string jsonContent = await Task.Run(() => JsonConvert.SerializeObject(standbyMode));
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await server.CreateClient()
                    .PostAsync(Entpoints.PostAddStandbyMode(), content);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Theory]
        [InlineData(BalanceTypes.Gold)]
        [InlineData(BalanceTypes.Money)]
        public async Task Post_delete_standbymode_and_response_ok_status_code(BalanceTypes balanceType)
        {
            using (var server = CreateServer())
            {
                StandbyModeModel standbyMode = new StandbyModeModel
                {
                    BalanceType = balanceType,
                    Bet = 1234,
                    ConnectionId = Guid.NewGuid().ToString(),
                    SubCategoryId = 1,
                };

                string jsonContent = await Task.Run(() => JsonConvert.SerializeObject(standbyMode));
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await server.CreateClient()
                    .PostAsync(Entpoints.PostDeleteStandbyMode(), content);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Theory]
        [InlineData(-1, 1, "abc")]
        [InlineData(1, -3456, "ro")]
        [InlineData(1, 1, "")]
        public async Task Post_delete_standbymode_and_response_badRequest_status_code(decimal bet, short subCategoryId, string connectionId)
        {
            using (var server = CreateServer())
            {
                StandbyModeModel standbyMode = new StandbyModeModel
                {
                    BalanceType = BalanceTypes.Gold,
                    Bet = bet,
                    ConnectionId = connectionId,
                    SubCategoryId = subCategoryId,
                };

                string jsonContent = await Task.Run(() => JsonConvert.SerializeObject(standbyMode));
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await server.CreateClient()
                    .PostAsync(Entpoints.PostDeleteStandbyMode(), content);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }
    }
}
