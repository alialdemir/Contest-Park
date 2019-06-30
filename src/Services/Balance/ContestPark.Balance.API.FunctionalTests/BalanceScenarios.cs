using ContestPark.Balance.API.Enums;
using ContestPark.Balance.API.Models;
using ContestPark.Core.FunctionalTests;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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
        public async Task Get_balance_and_check_amount()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetBalance());

                string responseContent = await response.Content.ReadAsStringAsync();

                BalanceModel balance = JsonConvert.DeserializeObject<BalanceModel>(responseContent);

                Assert.NotNull(balance);

                Assert.Equal(500000, balance.Money);
                Assert.Equal(100000, balance.Gold);
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

                BalanceResponse balance = JsonConvert.DeserializeObject<BalanceResponse>(responseContent);

                Assert.Equal(10000, balance.Amount);
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

                BalanceResponse balance = JsonConvert.DeserializeObject<BalanceResponse>(responseContent);

                Assert.Equal(5432, balance.Amount);
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

        [Fact]
        public async Task Post_purche_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                PurchaseModel purchase = new PurchaseModel
                {
                    PackageName = "com.contestparkapp.app.20000coins",
                    ProductId = "dsaads",
                    Token = "adsdsaddsaads",
                    Platform = Platforms.Android
                };

                string jsonContent = await Task.Run(() => JsonConvert.SerializeObject(purchase));
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await server.CreateClient()
                    .PostAsync(Entpoints.PostBalance(), content);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Theory]
        [InlineData("en-US", "Invalid platform type.")]
        [InlineData("tr-TR", "Geçersiz platform tipi.")]
        public async Task Post_purche_and_response_invalid_platform_type(string langCode, string errorMessage)
        {
            using (var server = CreateServer())
            {
                PurchaseModel purchase = new PurchaseModel
                {
                    PackageName = "com.contestparkapp.app.20000coins",
                    ProductId = "dsaads",
                    Token = "adsdsaddsaads",
                    Platform = (Platforms)1
                };

                string jsonContent = await Task.Run(() => JsonConvert.SerializeObject(purchase));
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await server.CreateClient()
                    .AddLangCode(langCode)
                    .PostAsync(Entpoints.PostBalance(), content);

                string responseContent = await response.Content.ReadAsStringAsync();

                Dictionary<string, string[]> errors = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(responseContent);

                Assert.Equal(errorMessage, errors.FirstOrDefault().Value.FirstOrDefault());
            }
        }

        [Theory]
        [InlineData("en-US", "Package name is incorrect.")]
        [InlineData("tr-TR", "Paket adı hatalı.")]
        public async Task Post_purche_and_response_invalid_package_name(string langCode, string errorMessage)
        {
            using (var server = CreateServer())
            {
                PurchaseModel purchase = new PurchaseModel
                {
                    PackageName = "fake-package-name",
                    ProductId = "dsaads",
                    Token = "adsdsaddsaads",
                    Platform = Platforms.Android
                };

                string jsonContent = await Task.Run(() => JsonConvert.SerializeObject(purchase));
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await server.CreateClient()
                    .AddLangCode(langCode)
                    .PostAsync(Entpoints.PostBalance(), content);

                string responseContent = await response.Content.ReadAsStringAsync();

                ValidationMessage errors = JsonConvert.DeserializeObject<ValidationMessage>(responseContent);

                Assert.Equal(errorMessage, errors.ErrorMessage);
            }
        }

        [Theory]
        [InlineData("com.contestparkapp.app.250coins", 100250)]
        [InlineData("com.contestparkapp.app.1500coins", 101750)]
        [InlineData("com.contestparkapp.app.7000coins", 108750)]
        [InlineData("com.contestparkapp.app.20000coins", 128750)]
        public async Task Post_purche_all_package_names_and_response_ok_status_code(string packageName, decimal balance)
        {
            using (var server = CreateServer())
            {
                PurchaseModel purchase = new PurchaseModel
                {
                    PackageName = packageName,
                    ProductId = "dsaads",
                    Token = "adsdsaddsaads",
                    Platform = Platforms.Android
                };

                string jsonContent = await Task.Run(() => JsonConvert.SerializeObject(purchase));
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var httpClient = server.CreateClient();

                var response = await httpClient.PostAsync(Entpoints.PostBalance(), content);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                #region Bakiye yükleme başarılı ise getBalance üzerinden gelen değeri kontrol ediyoruz

                var response2 = await httpClient.GetAsync(Entpoints.GetBalance());

                string responseContent = await response2.Content.ReadAsStringAsync();

                BalanceModel balanceModel = JsonConvert.DeserializeObject<BalanceModel>(responseContent);

                Assert.Equal(balance, balanceModel.Gold);

                #endregion Bakiye yükleme başarılı ise getBalance üzerinden gelen değeri kontrol ediyoruz
            }
        }

        private class BalanceResponse
        {
            public decimal Amount { get; set; }
        }
    }
}
