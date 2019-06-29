using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using System.Collections.Generic;
using System.Linq;

namespace ContestPark.Core.CosmosDb.Extensions
{
    public static class QueryableExtension
    {
        public static ServiceModel<TResult> ToServiceModel<TDocument, TResult>(this IRepository<TDocument> dbRepository, string sql, object parameters, PagingModel paging) where TDocument : class, IEntity<string>, new()
        {
            // Query olduğu gibi çalıştırıldı
            IEnumerable<TResult> items = dbRepository.QueryMultiple<TResult>($"{sql} OFFSET {paging.Offset} LIMIT {paging.PageSize}", parameters);

            ServiceModel<TResult> serviceModel = new ServiceModel<TResult>
            {
                Items = items,
                PageNumber = paging.PageNumber,
                PageSize = paging.PageSize,
            };

            if (items == null || items.Count() == 0)// Kayıt sayısı 0 ise sonraki sayfa var mı kontrol etmeye gerek yok
                return serviceModel;

            int whereQueryIndex = sql.ToLowerInvariant().IndexOf("where");// where kısmının indexi alındı
            string whereQuery = "";
            if (whereQueryIndex != -1)// where koşulu varsa
            {
                whereQuery = sql.Substring(whereQueryIndex + 5);// koşul kısmından sonrası alındı
            }

            serviceModel.HasNextPage = dbRepository.QuerySingle<bool>($"SELECT VALUE ({paging.PageSize} * ({paging.Offset} + 1)) < COUNT(c.id) FROM c WHERE {whereQuery}", parameters);// sonraki sayfa var mı kontrolü yapıldı

            return serviceModel;
        }

        public static IEnumerable<TSource> ToPaging<TSource>(this IEnumerable<TSource> enumerable, PagingModel pagingModel)
        {
            return enumerable
                .Skip(pagingModel.PageSize * (pagingModel.PageNumber - 1))
                .Take(pagingModel.PageSize);
        }
    }
}
