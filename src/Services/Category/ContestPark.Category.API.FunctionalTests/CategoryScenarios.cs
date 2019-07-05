using ContestPark.Category.API.IntegrationEvents.Events;
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
    public class CategoryScenarios : CategoryScenariosBase
    {
        [Fact]
        public async Task Get_all_categories_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.Get());

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Get_all_categories_and_response_notfound_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.Get(true, 10, 3));

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
        [InlineData("tr-TR", "Futbol", "Takımlar")]
        [InlineData("en-US", "Football", "Teams")]
        [InlineData("fakelangoagecode", "Football", "Teams")]
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
        [InlineData("tr-TR", "Stadyum")]
        [InlineData("en-US", "Stadium")]
        [InlineData("fakelangoagecode", "Stadium")]
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
                Assert.Equal(2, categories.Items.Count());

                SubCategoryModel subCategoryFirstItem = categories.Items.First();

                Assert.Equal(subcategoryName, subCategoryFirstItem.SubCategoryName);
                Assert.Equal("https://cdn2.iconfinder.com/data/icons/location-map-vehicles/100/Locations-53-512.png", subCategoryFirstItem.PicturePath);
                Assert.Null(subCategoryFirstItem.DisplayPrice);
                Assert.Equal(0, subCategoryFirstItem.Price);
            }
        }

        [Fact]
        public async Task Post_follow_subcategories_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                                           .PostAsync(Entpoints.PostFollowSubCategories(1), null);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Post_follow_subcategories_and_subcategoryid_zero_and_response_badrequest_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                                           .PostAsync(Entpoints.PostFollowSubCategories(0), null);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Theory, TestPriority(1)]
        [InlineData(4, "You are already following this category.")]
        [InlineData(3, "To be able to follow this category, you need to unlock it.")]
        public async Task Post_follow_subcategories_and_followed_by_subcategory_and_response_error_message(short subCategoryId, string message)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                                           .PostAsync(Entpoints.PostFollowSubCategories(subCategoryId), null);

                string responseContent = await response.Content.ReadAsStringAsync();

                ValidationMessage validation = JsonConvert.DeserializeObject<ValidationMessage>(responseContent);

                Assert.Equal(message, validation.ErrorMessage);
            }
        }

        [Fact]
        public async Task Delete_unfollow_subcategories_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                                           .DeleteAsync(Entpoints.DeleteUnFollowSubCategories(2));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Delete_unfollow_subcategories_and_fake_id_response_message_check_and_badrequest_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                                           .DeleteAsync(Entpoints.DeleteUnFollowSubCategories(88));

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                string responseContent = await response.Content.ReadAsStringAsync();

                ValidationMessage validation = JsonConvert.DeserializeObject<ValidationMessage>(responseContent);

                Assert.Equal("You must follow this category to deactivate the category.", validation.ErrorMessage);
            }
        }

        [Fact, TestPriority(2)]
        public async Task Get_subcategory_check_follow_status_and_response_true()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                                           .GetAsync(Entpoints.GetFollowStatus(4));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                string responseContent = await response.Content.ReadAsStringAsync();

                FollowStatus followStatus = JsonConvert.DeserializeObject<FollowStatus>(responseContent);

                Assert.True(followStatus.IsSubCategoryFollowed);
            }
        }

        [Fact]
        public async Task Get_not_follow_subcategory_check_follow_status_and_response_false()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                                           .GetAsync(Entpoints.GetFollowStatus(1));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                string responseContent = await response.Content.ReadAsStringAsync();

                FollowStatus followStatus = JsonConvert.DeserializeObject<FollowStatus>(responseContent);

                Assert.False(followStatus.IsSubCategoryFollowed);
            }
        }

        [Fact]
        public async Task Get_subcategory_detail_and_response_notfound_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Entpoints.GetSubcategoryDetail(65));

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Theory]
        [InlineData("tr-TR", 1, "Hakem", "Hakem açıklama", "https://static.thenounproject.com/png/14039-200.png")]
        [InlineData("en-US", 1, "Referee", "Referee description", "https://static.thenounproject.com/png/14039-200.png")]
        [InlineData("tr-TR", 2, "Stadyum", "Stadyum açıklama", "https://cdn2.iconfinder.com/data/icons/location-map-vehicles/100/Locations-53-512.png")]
        [InlineData("en-US", 2, "Stadium", "Stadium description", "https://cdn2.iconfinder.com/data/icons/location-map-vehicles/100/Locations-53-512.png")]
        public async Task Get_subcategory_detail_and_response_data_check(string langCode,
                                                                         short subCategoryId,
                                                                         string subCategoryName,
                                                                         string description,
                                                                         string subCategoryPicturePath)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                                           .AddLangCode(langCode)
                                           .GetAsync(Entpoints.GetSubcategoryDetail(subCategoryId));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                string responseContent = await response.Content.ReadAsStringAsync();

                SubCategoryDetailInfoModel subCategoryDetail = JsonConvert.DeserializeObject<SubCategoryDetailInfoModel>(responseContent);

                Assert.Equal(subCategoryId, subCategoryDetail.SubCategoryId);
                Assert.Equal(subCategoryName, subCategoryDetail.SubCategoryName);
                Assert.Equal(description, subCategoryDetail.Description);
                Assert.Equal(subCategoryPicturePath, subCategoryDetail.PicturePath);
            }
        }

        [Fact, TestPriority(2)]
        public async Task Post_unlock_subcategory_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                                           .PostAsync(Entpoints.PostUnLockSubcategory(3, BalanceTypes.Gold), null);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Theory, TestPriority(1)]
        [InlineData("en-US", "This category is already unlocked.")]
        [InlineData("tr-TR", "Bu kategorinin kilidi zaten açık.")]
        public async Task Post_unlock_subcategory_and_response_badrequest_status_code_and_error_message(string langCode, string message)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                                           .AddLangCode(langCode)
                                           .PostAsync(Entpoints.PostUnLockSubcategory(4, BalanceTypes.Money), null);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                string responseContent = await response.Content.ReadAsStringAsync();

                ValidationMessage validation = JsonConvert.DeserializeObject<ValidationMessage>(responseContent);

                Assert.Equal(message, validation.ErrorMessage);
            }
        }

        [Theory, TestPriority(1)]
        [InlineData("en-US", "You can not unlock the Free category.")]
        [InlineData("tr-TR", "Ücretsiz bir kategorinin kilidini açamazsınız.")]
        public async Task Post_unlock_free_subcategory_and_check_error_message(string langCode, string message)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                                           .AddLangCode(langCode)
                                           .PostAsync(Entpoints.PostUnLockSubcategory(2, BalanceTypes.Money), null);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                string responseContent = await response.Content.ReadAsStringAsync();

                ValidationMessage validation = JsonConvert.DeserializeObject<ValidationMessage>(responseContent);

                Assert.Equal(message, validation.ErrorMessage);
            }
        }
    }

    internal class ValidationMessage
    {
        public string ErrorMessage { get; set; }
    }

    public class FollowStatus
    {
        public bool IsSubCategoryFollowed { get; set; }
    }
}
