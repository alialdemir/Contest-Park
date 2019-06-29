using ContestPark.Category.API.Model;
using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using System.Threading.Tasks;

namespace ContestPark.Category.API.Infrastructure.Repositories.Search
{
    public interface ISearchRepository
    {
        void CreateSearchIndexs();

        void DeleteSearchIndexs();

        void Insert(Documents.Search searchModel);

        void Update(Documents.Search searchModel);

        Documents.Search SearchById(string id, SearchTypes searchType, Languages? language);

        Task<ServiceModel<SearchModel>> SearchFollowedSubCategoriesAsync(string searchText, string userId, Languages language, PagingModel pagingModel);

        Task<ServiceModel<SearchModel>> DynamicSearchAsync(string searchText, Languages language, PagingModel pagingModel, SearchFilters searchFilters, params string[] filterIds);
    }
}