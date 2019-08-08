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

        void Insert(Tables.Search searchModel);

        void Update(Tables.Search searchModel);

        Tables.Search SearchById(string id, SearchTypes searchType, Languages? language);

        Task<ServiceModel<SearchModel>> SearchFollowedSubCategoriesAsync(string searchText, string userId, Languages language, PagingModel pagingModel);

        Task<ServiceModel<SearchModel>> DynamicSearchAsync(string searchText, Languages language, string userId, PagingModel pagingModel, SearchFilters searchFilters, params short[] filterIds);
    }
}
