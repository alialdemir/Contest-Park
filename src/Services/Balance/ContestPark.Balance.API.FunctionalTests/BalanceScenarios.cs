using ContestPark.Balance.API.Enums;
using ContestPark.Balance.API.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ContestPark.Balance.API.FunctionalTests
{
    public class BalanceScenarios : BalanceScenariosBase
    {
        [Fact]
        public async Task Get_balance_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetBalance());

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Get_balance_and_check_balance_types()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetBalance());

                string responseContent = await response.Content.ReadAsStringAsync();

                IEnumerable<BalanceModel> balances = JsonConvert.DeserializeObject<IEnumerable<BalanceModel>>(responseContent);

                Assert.Equal(2, balances.Count());

                Assert.Equal(BalanceTypes.Gold, balances.FirstOrDefault().BalanceType);
                Assert.Equal(BalanceTypes.Money, balances.LastOrDefault().BalanceType);
            }
        }

        [Fact]
        public async Task Get_balance_and_check_amount()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetBalance());

                string responseContent = await response.Content.ReadAsStringAsync();

                IEnumerable<BalanceModel> balances = JsonConvert.DeserializeObject<IEnumerable<BalanceModel>>(responseContent);

                Assert.Equal(2, balances.Count());

                Assert.Equal(99999999, balances.FirstOrDefault().Amount);
                Assert.Equal(99999999, balances.LastOrDefault().Amount);
            }
        }

        [Fact]
        public async Task Get_balance_by_userid_and_check_gold_amount()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetBalanceByUserId("2222-2222-2222-2222", BalanceTypes.Gold));

                string responseContent = await response.Content.ReadAsStringAsync();

                BalanceModel balance = JsonConvert.DeserializeObject<BalanceModel>(responseContent);

                Assert.Equal(10000.0M, balance.Amount);
            }
        }

        [Fact]
        public async Task Get_balance_by_userid_and_check_money_amount()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetBalanceByUserId("2222-2222-2222-2222", BalanceTypes.Money));

                string responseContent = await response.Content.ReadAsStringAsync();

                BalanceModel balance = JsonConvert.DeserializeObject<BalanceModel>(responseContent);

                Assert.Equal(5432.0m, balance.Amount);
            }
        }

        [Fact]
        public async Task Get_balance_by_userid_and_balance_type_null_and_response_gold_amount()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetBalanceByUserId("2222-2222-2222-2222", null));

                string responseContent = await response.Content.ReadAsStringAsync();

                BalanceModel balance = JsonConvert.DeserializeObject<BalanceModel>(responseContent);

                Assert.Equal(10000.0M, balance.Amount);
            }
        }

        [Fact]
        public async Task Get_balance_by_userid_and_fake_userid_and_response_notfound()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetBalanceByUserId("fake-userid", null));

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
    }
}