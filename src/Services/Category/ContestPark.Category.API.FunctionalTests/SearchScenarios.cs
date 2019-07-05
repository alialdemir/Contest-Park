using ContestPark.Category.API.Model;
using ContestPark.Core.Database.Models;
using ContestPark.Core.FunctionalTests;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ContestPark.Category.API.FunctionalTests
{
    public class SearchScenarios : CategoryScenariosBase
    {
        [Fact]
        public async Task Get_search_followed_subcategories_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                await Task.Delay(500);
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetSearchFollowedSubCategories("Stadium"));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Theory]
        [InlineData("tr-TR", "Futbol", "Stadyum")]
        [InlineData("en-US", "Football", "Stadium")]
        [InlineData("fakelangoagecode", "Football", "Stadium")]
        public async Task Get_search_followed_subcategories_and_data_check(string languageCode, string categoryName, string subcategoryName)
        {
            using (var server = CreateServer())
            {
                await Task.Delay(900);

                var response = await server.CreateClient()
                                                    .AddLangCode(languageCode)
                                                    .GetAsync(Entpoints.GetSearchFollowedSubCategories(subcategoryName));

                string responseContent = await response.Content.ReadAsStringAsync();

                ServiceModel<SearchModel> search = JsonConvert.DeserializeObject<ServiceModel<SearchModel>>(responseContent);

                SearchModel subCategoryFirstItem = search.Items.First();
                Assert.NotNull(search);
                Assert.Single(search.Items);

                Assert.NotNull(subCategoryFirstItem);

                Assert.Equal(subcategoryName, subCategoryFirstItem.SubCategoryName);
                Assert.Equal(categoryName, subCategoryFirstItem.CategoryName);
                Assert.Equal("https://cdn2.iconfinder.com/data/icons/location-map-vehicles/100/Locations-53-512.png", subCategoryFirstItem.PicturePath);
                Assert.Empty(subCategoryFirstItem.DisplayPrice);
                Assert.Equal(0, subCategoryFirstItem.Price);
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData("kayıtlı olmayan bir arama")]
        public async Task Get_search_followed_subcategories_and_null_query_response_notfound_status_code(string searchText)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                                                    .GetAsync(Entpoints.GetSearchFollowedSubCategories(searchText));

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task Get_search_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetSearch(1, "Referee"));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Theory]
        [InlineData("en-US", "r", "Referee")]
        [InlineData("en-US", "re", "Referee")]
        [InlineData("en-US", "ref", "Referee")]
        [InlineData("tr-TR", "h", "Hakem")]
        [InlineData("tr-TR", "hak", "Hakem")]
        [InlineData("tr-TR", "HaKe", "Hakem")]
        public async Task Get_search_categoryid_and_check_subcategoryname(string languageCode, string searchText, string result)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                                           .AddLangCode(languageCode)
                                           .GetAsync(Entpoints.GetSearch(1, searchText));

                string responseContent = await response.Content.ReadAsStringAsync();

                ServiceModel<SearchModel> search = JsonConvert.DeserializeObject<ServiceModel<SearchModel>>(responseContent);

                SearchModel subCategoryFirstItem = search.Items.First();

                Assert.Equal(result, subCategoryFirstItem.SubCategoryName);
            }
        }

        [Theory]
        [InlineData("en-US", "f", "Football")]
        [InlineData("en-US", "foo", "Football")]
        [InlineData("en-US", "foot", "Football")]
        [InlineData("tr-TR", "f", "Futbol")]
        [InlineData("tr-TR", "fUt", "Futbol")]
        [InlineData("tr-TR", "futb", "Futbol")]
        public async Task Get_search_categoryid_and_check_categoryname(string languageCode, string searchText, string result)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                                           .AddLangCode(languageCode)
                                           .GetAsync(Entpoints.GetSearch(1, searchText));

                string responseContent = await response.Content.ReadAsStringAsync();

                ServiceModel<SearchModel> search = JsonConvert.DeserializeObject<ServiceModel<SearchModel>>(responseContent);

                SearchModel subCategoryFirstItem = search.Items.First();

                Assert.Equal(result, subCategoryFirstItem.CategoryName);
            }
        }

        [Fact]
        public async Task Get_search_categoryid_and_searchtext_empty_all_data()
        {
            using (var server = CreateServer())
            {
                await Task.Delay(1000);// Kategori dataları elasticsearch e yüklenemesini bekletmek için 5sn beklettim

                var response = await server.CreateClient()
                                           .GetAsync(Entpoints.GetSearch(1, ""));

                string responseContent = await response.Content.ReadAsStringAsync();

                ServiceModel<SearchModel> search = JsonConvert.DeserializeObject<ServiceModel<SearchModel>>(responseContent);

                Assert.Equal(4, search.Items.Count());
            }
        }

        [Fact]
        public async Task Get_all_search_and_searchtext_empty_all_data()
        {
            using (var server = CreateServer())
            {
                await Task.Delay(800);// Kategori dataları elasticsearch e yüklenemesini bekletmek için 5sn beklettim

                var response = await server.CreateClient()
                                           .GetAsync(Entpoints.GetSearch(0, ""));

                string responseContent = await response.Content.ReadAsStringAsync();

                ServiceModel<SearchModel> search = JsonConvert.DeserializeObject<ServiceModel<SearchModel>>(responseContent);

                Assert.Equal(7, search.Items.Count());
            }
        }

        [Theory]
        [InlineData("w", "witcherfearless", "Ali Aldemir")]
        [InlineData("wit", "witcherfearless", "Ali Aldemir")]
        [InlineData("witcher", "witcherfearless", "Ali Aldemir")]
        [InlineData("ali", "witcherfearless", "Ali Aldemir")]
        [InlineData("b", "bot12345", "Bot")]
        [InlineData("bo", "bot12345", "Bot")]
        [InlineData("bot", "bot12345", "Bot")]
        [InlineData("d", "demo", "Demo")]
        [InlineData("de", "demo", "Demo")]
        [InlineData("dem", "demo", "Demo")]
        [InlineData("demo", "demo", "Demo")]
        public async Task Get_user_search_and_check_user_data(string searchText, string userName, string fullName)
        {
            using (var server = CreateServer())
            {
                await Task.Delay(500);// Kategori dataları elasticsearch e yüklenemesini bekletmek için 5sn beklettim

                var response = await server.CreateClient()
                                           .GetAsync(Entpoints.GetSearch(0, searchText));

                string responseContent = await response.Content.ReadAsStringAsync();

                ServiceModel<SearchModel> search = JsonConvert.DeserializeObject<ServiceModel<SearchModel>>(responseContent);

                SearchModel FirstItem = search.Items.FirstOrDefault();

                Assert.Equal(FirstItem.FullName, fullName);
                Assert.Equal(FirstItem.UserName, userName);
            }
        }
    }
}
