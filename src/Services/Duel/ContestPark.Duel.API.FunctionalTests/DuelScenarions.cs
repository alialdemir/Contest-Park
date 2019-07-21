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
                    .PostAsync(Entpoints.PostStandbyMode(), content);

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
                    .PostAsync(Entpoints.PostStandbyMode(), content);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }
    }
}
