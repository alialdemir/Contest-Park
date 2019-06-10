using ContestPark.Category.API.Model;
using ContestPark.Core.CosmosDb.Models;
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
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetSearchFollowedSubCategories("Referee"));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Theory]
        [InlineData("tr-TR", "Futbol", "Hakem")]
        [InlineData("en-US", "Football", "Referee")]
        [InlineData("fakelangoagecode", "Football", "Referee")]
        public async Task Get_search_followed_subcategories_and_data_check(string languageCode, string categoryName, string subcategoryName)
        {
            using (var server = CreateServer())
            {
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
                Assert.Equal("https://static.thenounproject.com/png/14039-200.png", subCategoryFirstItem.PicturePath);
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
    }
}