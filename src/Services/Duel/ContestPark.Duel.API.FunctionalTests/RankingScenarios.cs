using ContestPark.Duel.API.Enums;
using ContestPark.Duel.API.Models;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ContestPark.Duel.API.FunctionalTests
{
    public class RankingScenarios : DuelScenariosBase
    {
        [Theory]
        [InlineData(BalanceTypes.Gold)]
        [InlineData(BalanceTypes.Money)]
        public async Task Get_subcategory_ranks_and_response_ok_status_code(BalanceTypes balanceType)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetRankingBySubCategoryId(1, balanceType));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Theory]
        [InlineData(BalanceTypes.Gold)]
        [InlineData(BalanceTypes.Money)]
        public async Task Get_subcategory_ranks_and_response_badrequest_status_code(BalanceTypes balanceType)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetRankingBySubCategoryId(0, balanceType));

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Theory]
        [InlineData(BalanceTypes.Gold)]
        [InlineData(BalanceTypes.Money)]
        public async Task Get_subcategory_ranks_and_check_paging_values(BalanceTypes balanceType)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetRankingBySubCategoryId(1, balanceType, true, 1, 1));

                string responseContent = await response.Content.ReadAsStringAsync();

                RankingModel rankInfo = JsonConvert.DeserializeObject<RankingModel>(responseContent);

                Assert.NotNull(rankInfo);
                Assert.Equal(1, rankInfo.Ranks.PageNumber);
                Assert.Equal(1, rankInfo.Ranks.PageSize);
                Assert.Single(rankInfo.Ranks.Items);
                Assert.True(rankInfo.Ranks.HasNextPage);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
