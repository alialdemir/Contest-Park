using ContestPark.Core.CosmosDb.Models;

namespace ContestPark.Core.CosmosDb.Extensions
{
    public static class QueryableExtension
    {
        public static long PageNumberCalculate(PagingModel paging)
        {
            return paging.PageSize * (paging.PageNumber - 1);
        }
    }
}