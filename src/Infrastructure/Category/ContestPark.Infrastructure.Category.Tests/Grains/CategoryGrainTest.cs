using ContestPark.Core.Domain.Model;
using ContestPark.Core.Enums;
using ContestPark.Infrastructure.Category.Grains;
using ContestPark.Infrastructure.Category.Repositories.Category;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ContestPark.Infrastructure.Category.Tests
{
    public class CategoryGrainTest : TestGrainBase
    {
        /// <summary>
        /// Eğer user id boş giderse
        /// Count 0 olmalı
        /// Item null olmamalı
        /// Items sayısı sıfır olmalı
        /// PageNumber mock PageNumber ile aynı olmalı değişmemeli
        /// PageSize mock PageSize ile aynı olmalı değişmemeli
        /// </summary>
        [Fact]
        public async Task Should_Items_Count_Zero_When_UserId_Empty()
        {
            // Arrange
            var serviceResponseMock = new ServiceResponse<Domain.Category.Model.Response.Category>
            {
                PageNumber = Faker.RandomNumber.Next(),
                PageSize = Faker.RandomNumber.Next()
            };

            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            categoryRepositoryMock.Setup(x => x.GetCategoryList(It.IsAny<string>(), It.IsAny<Languages>(), It.IsAny<Paging>()))
                                  .Returns(serviceResponseMock);

            Silo.ServiceProvider.AddServiceProbe(categoryRepositoryMock);

            var grain = Silo.CreateGrain<CategoryGrain>(0);

            // Act
            var result = await grain.GetCategoryList(string.Empty, Languages.English, serviceResponseMock);

            // Assert
            Assert.Equal(0, result.Count);
            Assert.NotNull(result.Items);
            Assert.Equal(0, result.Items.ToList().Count);
            Assert.Equal(serviceResponseMock.PageNumber, result.PageNumber);
            Assert.Equal(serviceResponseMock.PageSize, result.PageSize);
        }

        /// <summary>
        /// Mock bir dummy datalar ile ServiceResponse modeli oluşturup  repositoryden alıyor
        /// grain içerisinde bu datalar değişiyor mu bunu test eder
        /// </summary>
        [Fact]
        public async Task Given_Mock_Items_Then_Result_Mock_Items()
        {
            // Arrange
            var serviceResponseMock = new ServiceResponse<Domain.Category.Model.Response.Category>()
            {
                Items = new List<Domain.Category.Model.Response.Category>
                {
                   new Domain.Category.Model.Response.Category
                   {
                       CategoryId = (short)Faker.RandomNumber.Next(short.MinValue, short.MaxValue),
                       CategoryName= Faker.Name.FullName(),
                       SubCategories = new List<Domain.Category.Model.Response.SubCategory>
                       {
                           new Domain.Category.Model.Response.SubCategory
                           {
                               DisplayPrice = Faker.RandomNumber.Next().ToString(),
                               PicturePath= Faker.Address.Country(),
                               Price = Faker.RandomNumber.Next(),
                               SubCategoryId =  (short)Faker.RandomNumber.Next(short.MinValue, short.MaxValue),
                               SubCategoryName= Faker.Name.Suffix()
                           }
                       }
                   }
                }
            };

            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            categoryRepositoryMock.Setup(x => x.GetCategoryList(It.IsAny<string>(), It.IsAny<Languages>(), It.IsAny<Paging>()))
                                  .Returns(serviceResponseMock);

            Silo.ServiceProvider.AddServiceProbe(categoryRepositoryMock);

            var grain = Silo.CreateGrain<CategoryGrain>(1);

            // Act
            var result = await grain.GetCategoryList(WitcherUserId, Languages.English, new Paging());

            // Assert
            var serviceItem = serviceResponseMock.Items.FirstOrDefault();
            var resultItem = result.Items.FirstOrDefault();

            // Count
            Assert.Equal(serviceResponseMock.Count, result.Count);
            Assert.Equal(serviceItem.SubCategories.Count, resultItem.SubCategories.Count);

            // Category items
            Assert.Equal(serviceResponseMock.Items.ToList().Count, result.Items.ToList().Count);
            Assert.Equal(serviceItem.CategoryId, resultItem.CategoryId);
            Assert.Equal(serviceItem.CategoryName, resultItem.CategoryName);

            //Sub Categories
            var servicesubCategoryItem = serviceResponseMock.Items.FirstOrDefault().SubCategories.FirstOrDefault();
            var subCategoryItem = resultItem.SubCategories.FirstOrDefault();

            Assert.Equal(servicesubCategoryItem.DisplayPrice, subCategoryItem.DisplayPrice);
            Assert.Equal(servicesubCategoryItem.PicturePath, subCategoryItem.PicturePath);
            Assert.Equal(servicesubCategoryItem.Price, subCategoryItem.Price);
            Assert.Equal(servicesubCategoryItem.SubCategoryId, subCategoryItem.SubCategoryId);
            Assert.Equal(servicesubCategoryItem.SubCategoryName, subCategoryItem.SubCategoryName);
        }
    }
}