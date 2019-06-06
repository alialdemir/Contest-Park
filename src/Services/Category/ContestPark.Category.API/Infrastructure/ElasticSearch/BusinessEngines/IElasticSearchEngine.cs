using System.Collections.Generic;

namespace ContestPark.Category.API.Infrastructure.ElasticSearch.BusinessEngines
{
    public interface IElasticSearchEngine
    {
        List<T> Execute<T>() where T : class;
    }
}