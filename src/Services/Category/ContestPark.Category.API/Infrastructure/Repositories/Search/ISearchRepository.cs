using ContestPark.Category.API.Model;
using ContestPark.Core.CosmosDb.Models;
using ContestPark.Core.Enums;
using System.Threading.Tasks;

namespace ContestPark.Category.API.Infrastructure.Repositories.Search
{
    public interface ISearchRepository
    {
        void CreateCategoryIndex();

        void Insert(Documents.Search searchModel);

        void Update(Documents.Search searchModel);

        Documents.Search SearchById(string id, SearchTypes searchType, Languages? language);

        Task<ServiceModel<SearchModel>> GetFollowedSubCategories(string searchText, string userId, Languages language, PagingModel pagingModel);
    }
}