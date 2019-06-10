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
    public class CategoryScenarios : CategoryScenariosBase
    {
        [Fact]
        public async Task Get_get_all_categories_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.Get());

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Get_get_all_categories_and_response_notfound_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.Get(true, 10, 2));

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task Get_categories_and_check_paging_values()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.Get(true, 1, 1));

                string responseContent = await response.Content.ReadAsStringAsync();

                ServiceModel<CategoryModel> categories = JsonConvert.DeserializeObject<ServiceModel<CategoryModel>>(responseContent);

                Assert.NotNull(categories);
                Assert.Equal(1, categories.PageNumber);
                Assert.Equal(1, categories.PageSize);
                Assert.Single(categories.Items);
                Assert.False(categories.HasNextPage);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Theory]
        [InlineData("tr-TR", "Futbol", "Hakem")]
        [InlineData("en-US", "Football", "Referee")]
        [InlineData("fakelangoagecode", "Football", "Referee")]
        public async Task Get_categories_and_check_language(string languageCode, string categoryName, string subcategoryName)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .AddLangCode(languageCode)
                    .GetAsync(Entpoints.Get());

                string responseContent = await response.Content.ReadAsStringAsync();

                ServiceModel<CategoryModel> categories = JsonConvert.DeserializeObject<ServiceModel<CategoryModel>>(responseContent);

                Assert.NotNull(categories);
                Assert.Equal(categoryName, categories.Items.First().CategoryName);
                Assert.Equal(subcategoryName, categories.Items.First().SubCategories.First().SubCategoryName);
            }
        }

        [Fact]
        public async Task Get_followed_subcategories_and_response_notfound_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetFollowedSubCategories(true, 10, 2));

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Theory]
        [InlineData("tr-TR", "Hakem")]
        [InlineData("en-US", "Referee")]
        [InlineData("fakelangoagecode", "Referee")]
        public async Task Get_followed_subcategories_and_check_language(string languageCode, string subcategoryName)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .AddLangCode(languageCode)
                    .GetAsync(Entpoints.GetFollowedSubCategories());

                string responseContent = await response.Content.ReadAsStringAsync();

                ServiceModel<SubCategoryModel> categories = JsonConvert.DeserializeObject<ServiceModel<SubCategoryModel>>(responseContent);

                Assert.NotNull(categories);
                Assert.Single(categories.Items);

                SubCategoryModel subCategoryFirstItem = categories.Items.First();

                Assert.Equal(subcategoryName, subCategoryFirstItem.SubCategoryName);
                Assert.Equal("https://static.thenounproject.com/png/14039-200.png", subCategoryFirstItem.PicturePath);
                Assert.Empty(subCategoryFirstItem.DisplayPrice);
                Assert.Equal(0, subCategoryFirstItem.Price);
            }
        }
    }
}