using ContestPark.Domain.Category.Interfaces;
using Nest;
using Orleans;
using System;
using System.Threading.Tasks;

namespace ContestPark.Infrastructure.Category.Grains
{
    public class CategorySearchGrain : Grain, ICategorySearchGrain
    {
        private readonly IElasticClient _elasticClient;

        public CategorySearchGrain(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task Search()
        {
            string query = "lorem";
            int page = 1;
            int pageSize = 5;

            var response = await _elasticClient.SearchAsync<Post>(
               s => s.Query(q => q.QueryString(d => d.Query(query)))
                   .From((page - 1) * pageSize)
                   .Size(pageSize));

            if (!response.IsValid)
            {
                // We could handle errors here by checking response.OriginalException or response.ServerError properties
            }

            var doc = response.Documents;
        }

        private static string GetSearchUrl(string query, int page, int pageSize)
        {
            return $"/search?query={Uri.EscapeDataString(query ?? "")}&page={page}&pagesize={pageSize}/";
        }
    }
}