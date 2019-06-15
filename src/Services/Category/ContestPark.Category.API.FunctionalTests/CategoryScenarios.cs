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

        [Fact]
        public async Task Post_follow_subcategories_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                                           .PostAsync(Entpoints.PostFollowSubCategories("7c3a26b7-74df-4128-aab9-a21f81a5ab36"), null);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Post_follow_subcategories_and_subcategoryid_null_and_response_notfound_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                                           .PostAsync(Entpoints.PostFollowSubCategories(""), null);

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Theory]
        [InlineData("9d15a162-9ffc-42aa-91dc-d7f02b6f0080", "You are already following this category.")]
        [InlineData("24461fb6-323d-43e6-9a85-b263cff51bcc", "To be able to follow this category, you need to unlock it.")]
        public async Task Post_follow_subcategories_and_followed_by_subcategory_and_response_error_message(string subCategoryId, string message)
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
                                           .DeleteAsync(Entpoints.DeleteUnFollowSubCategories("9d15a162-9ffc-42aa-91dc-d7f02b6f0080"));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Delete_unfollow_subcategories_and_fake_id_response_message_check_and_badrequest_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                                           .DeleteAsync(Entpoints.DeleteUnFollowSubCategories("fakeid"));

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                string responseContent = await response.Content.ReadAsStringAsync();

                ValidationMessage validation = JsonConvert.DeserializeObject<ValidationMessage>(responseContent);

                Assert.Equal("You must follow this category to deactivate the category.", validation.ErrorMessage);
            }
        }

        [Fact]
        public async Task Get_subcategory_check_follow_status_and_response_true()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                                           .GetAsync(Entpoints.GetFollowStatus("9d15a162-9ffc-42aa-91dc-d7f02b6f0080"));

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
                                           .GetAsync(Entpoints.GetFollowStatus("24461fb6-323d-43e6-9a85-b263cff51bcc"));

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
                    .GetAsync(Entpoints.GetSubcategoryDetail("fake-subcategoryid"));

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Theory]
        [InlineData("tr-TR", "9d15a162-9ffc-42aa-91dc-d7f02b6f0080", "Hakem", "açıklama bla bla bla", "https://static.thenounproject.com/png/14039-200.png")]
        [InlineData("en-US", "9d15a162-9ffc-42aa-91dc-d7f02b6f0080", "Referee", "açıklama bla bla bla", "https://static.thenounproject.com/png/14039-200.png")]
        [InlineData("tr-TR", "7c3a26b7-74df-4128-aab9-a21f81a5ab36", "Stadyum", "açıklama bla bla bla", "https://cdn2.iconfinder.com/data/icons/location-map-vehicles/100/Locations-53-512.png")]
        [InlineData("en-US", "7c3a26b7-74df-4128-aab9-a21f81a5ab36", "Stadium", "açıklama bla bla bla", "https://cdn2.iconfinder.com/data/icons/location-map-vehicles/100/Locations-53-512.png")]
        public async Task Get_subcategory_detail_and_response_data_check(string langCode,
                                                                         string subCategoryId,
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

                SubCategoryDetailModel subCategoryDetail = JsonConvert.DeserializeObject<SubCategoryDetailModel>(responseContent);

                Assert.Equal(subCategoryId, subCategoryDetail.SubCategoryId);
                Assert.Equal(subCategoryName, subCategoryDetail.SubCategoryName);
                Assert.Equal(description, subCategoryDetail.Description);
                Assert.Equal(subCategoryPicturePath, subCategoryDetail.SubCategoryPicturePath);
            }
        }

        [Fact]
        public async Task Post_unlock_subcategory_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                                           .PostAsync(Entpoints.PostUnLockSubcategory("24461fb6-323d-43e6-9a85-b263cff51bcc"), null);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Theory]
        [InlineData("en-US", "This category is already unlocked.")]
        [InlineData("tr-TR", "Bu kategorinin kilidi zaten açık.")]
        public async Task Post_unlock_subcategory_and_response_badrequest_status_code_and_error_message(string langCode, string message)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                                           .AddLangCode(langCode)
                                           .PostAsync(Entpoints.PostUnLockSubcategory("9d15a162-9ffc-42aa-91dc-d7f02b6f0080"), null);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                string responseContent = await response.Content.ReadAsStringAsync();

                ValidationMessage validation = JsonConvert.DeserializeObject<ValidationMessage>(responseContent);

                Assert.Equal(message, validation.ErrorMessage);
            }
        }

        [Theory]
        [InlineData("en-US", "You can not unlock the Free category.")]
        [InlineData("tr-TR", "Ücretsiz bir kategorinin kilidini açamazsınız.")]
        public async Task Post_unlock_free_subcategory_and_check_error_message(string langCode, string message)
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                                           .AddLangCode(langCode)
                                           .PostAsync(Entpoints.PostUnLockSubcategory("7c3a26b7-74df-4128-aab9-a21f81a5ab36"), null);

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