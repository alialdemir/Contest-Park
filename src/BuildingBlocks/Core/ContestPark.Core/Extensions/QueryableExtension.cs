using ContestPark.Core.Domain.Model;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace ContestPark.Core.Extensions
{
    public static class QueryableExtension
    {
        /// <summary>
        /// IFindFluent tipindeki query'leri apiden dönüş yapabilecek türde(ServiceModel) dönüştürür
        /// </summary>
        /// <typeparam name="TSource">Generic model type</typeparam>
        /// <param name="source">query</param>
        /// <param name="paging">Paging</param>
        /// <returns>ServiceModel<TSource> service result</returns>
        public static ServiceResponse<TSource> QueryPaging<TSource>(this IDbConnection source, string query, object param, Paging paging)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (paging == null)
                throw new ArgumentNullException(nameof(paging));

            dynamic paramss = CreateExpandoFromObject(param);
            paramss.PageSize = paging.PageSize;
            paramss.PageNumber = PageNumberCalculate(paging);

            string percent = "SELECT TOP (100) PERCENT " + query.Substring(6);
            string query1 = $@"SELECT COUNT(*) FROM ({percent}) AS c;",
                   query2 = query + " OFFSET @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY";
            var queryMultiple = source.QueryMultiple(query1 + query2, (object)paramss);

            ServiceResponse<TSource> serviceModel = new ServiceResponse<TSource>
            {
                Count = queryMultiple.Read<int>().FirstOrDefault(),
                PageSize = paging.PageSize,
                PageNumber = paging.PageNumber,
            };

            if (IsGetData(serviceModel.Count, paging))
                serviceModel.Items = queryMultiple.Read<TSource>();

            return serviceModel;
        }

        public static long PageNumberCalculate(Paging paging)
        {
            return paging.PageSize * (paging.PageNumber - 1);
        }

        private static ExpandoObject CreateExpandoFromObject(object source)
        {
            var result = new ExpandoObject();
            if (source == null) return result;

            IDictionary<string, object> dictionary = result;
            foreach (var property in source
                .GetType()
                .GetProperties()
                .Where(p => p.CanRead && p.GetMethod.IsPublic))
            {
                dictionary[property.Name] = property.GetValue(source, null);
            }
            return result;
        }

        public static bool IsGetData(long count, Paging paging)
        {
            if (paging.PageNumber <= 0 || paging.PageSize <= 0) return false;
            return !((paging.PageNumber - 1) > count / paging.PageSize);
        }
    }
}