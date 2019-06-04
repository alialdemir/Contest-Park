using ContestPark.Category.API.Infrastructure.Documents;
using ContestPark.Core.CosmosDb.Interfaces;

namespace ContestPark.Category.API.Infrastructure.Repositories.OpenCategory
{
    public interface IOpenCategoryRepository
    {
        IDocumentDbRepository<OpenSubCategory> Repository { get; }

        string[] OpenSubCategoryIds(string userId);
    }
}