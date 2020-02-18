using ContestPark.Core.Database.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ContestPark.Core.Database.Interfaces
{
    public interface IQueryRepository
    {
        IEnumerable<T> QueryMultiple<T>(string sql, object parameters = null, CommandType? commandType = null);

        T QuerySingleOrDefault<T>(string sql, object parameters = null, CommandType? commandType = null);

        IEnumerable<TThird> SpQuery<TFirst, TSecond, TThird>(string sql, Func<TFirst, TSecond, TThird> map, object parameters = null, string splitOn = "", CommandType? commandType = null);

        Task<bool> ExecuteAsync(string sql, object parameters = null, CommandType? commandType = null);

        ServiceModel<TResult> ToServiceModel<TResult>(string sql, object parameters = null, CommandType? commandType = null, PagingModel pagingModel = null);

        ServiceModel<TResult> ToSpServiceModel<TResult>(string sql, object parameters = null, PagingModel pagingModel = null);
    }
}
