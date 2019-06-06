using ContestPark.Category.API.Model;

namespace ContestPark.Category.API.Infrastructure.Repositories.Search
{
    public interface ISearchRepository
    {
        void CreateCategoryIndex();

        void Insert(SearchModel searchModel);

        void Update(SearchModel searchModel);

        SearchModel SearchByUserId(string userId);
    }
}