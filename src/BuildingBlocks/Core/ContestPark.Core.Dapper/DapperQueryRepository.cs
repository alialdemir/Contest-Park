using ContestPark.Core.Dapper.Abctract;
using ContestPark.Core.Dapper.Interfaces;
using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Core.Dapper
{
    public class DapperQueryRepository : IQueryRepository
    {
        #region Private variables

        internal readonly IDatabaseConnection _databaseConnection;

        internal readonly ILogger<DapperQueryRepository> _logger;

        #endregion Private variables

        #region Constructor

        public DapperQueryRepository(IDatabaseConnection databaseConnection,
                                ILogger<DapperQueryRepository> logger)
        {
            _databaseConnection = databaseConnection;

            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.MySQL);

            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
        }

        #endregion Constructor

        #region Methods

        public IEnumerable<T> QueryMultiple<T>(string sql, object parameters = null, CommandType? commandType = null)
        {
            var result = _databaseConnection.Connection.QueryMultiple(sql, parameters, commandType: commandType).Read<T>();

            ((Disposable)_databaseConnection)?.DisposeCore();

            return result;
        }

        public T QuerySingleOrDefault<T>(string sql, object parameters = null, CommandType? commandType = null)
        {
            var result = _databaseConnection.Connection.QuerySingleOrDefault<T>(sql, parameters, commandType: commandType);
            ((Disposable)_databaseConnection)?.DisposeCore();

            return result;
        }

        public IEnumerable<TThird> SpQuery<TFirst, TSecond, TThird>(string sql, Func<TFirst, TSecond, TThird> map, object parameters = null, string splitOn = "", CommandType? commandType = null)
        {
            var result = _databaseConnection.Connection.Query<TFirst, TSecond, TThird>(sql, map, parameters, splitOn: splitOn, commandType: commandType);

            ((Disposable)_databaseConnection)?.DisposeCore();

            return result;
        }

        /// <summary>
        /// Sp çalıştırır
        /// </summary>
        /// <param name="sql">çalıştırılacak sql</param>
        /// <param name="parameters">Parametreleri</param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public async Task<bool> ExecuteAsync(string sql, object parameters = null, CommandType? commandType = null)
        {
            try
            {
                int result = await _databaseConnection.Connection.ExecuteAsync(sql, parameters, commandType: commandType);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"sql query execute edilirken hata oluştu.");
                return false;
            }
            finally
            {
                ((Disposable)_databaseConnection)?.DisposeCore();
            }
        }

        /// <summary>
        /// Sayfalama yaparak geriye döndürür
        /// </summary>
        /// <typeparam name="TResult">Return modeli</typeparam>
        /// <param name="sql">Sql query</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Service model</returns>
        public ServiceModel<TResult> ToServiceModel<TResult>(string sql, object parameters = null, CommandType? commandType = null, PagingModel pagingModel = null)
        {
            dynamic paramss = CreateExpandoFromObject(parameters);
            paramss.PageSize = pagingModel.PageSize;
            paramss.Offset = pagingModel.Offset;
            paramss.Offset2 = NextOffset(pagingModel.PageSize, pagingModel.PageNumber);

            string query1 = sql + " LIMIT @Offset2, @PageSize; ";
            string query2 = sql + " LIMIT @Offset, @PageSize";

            IEnumerable<TResult> nextPageCount = QueryMultiple<TResult>(query1, (object)paramss);

            ServiceModel<TResult> serviceModel = new ServiceModel<TResult>
            {
                PageSize = pagingModel.PageSize,
                PageNumber = pagingModel.PageNumber,
                HasNextPage = nextPageCount.Any()
            };

            serviceModel.Items = QueryMultiple<TResult>(query2, (object)paramss);

            return serviceModel;
        }

        /// <summary>
        /// StoredProcedure ile sayfalama yaparak çekme
        /// </summary>
        /// <typeparam name="TResult">Return modeli</typeparam>
        /// <param name="sql">Sql query</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Service model</returns>
        public ServiceModel<TResult> ToServiceModel<TResult>(string sql, object parameters = null, PagingModel pagingModel = null)
        {
            dynamic paramss = CreateExpandoFromObject(parameters);
            paramss.PageSize = pagingModel.PageSize;
            paramss.Offset = NextOffset(pagingModel.PageSize, pagingModel.PageNumber);

            IEnumerable<TResult> nextPageCount = QueryMultiple<TResult>(sql, paramss, commandType: CommandType.StoredProcedure);

            ServiceModel<TResult> serviceModel = new ServiceModel<TResult>
            {
                PageSize = pagingModel.PageSize,
                PageNumber = pagingModel.PageNumber,
                HasNextPage = nextPageCount.Any()
            };

            paramss.Offset = pagingModel.Offset;

            serviceModel.Items = QueryMultiple<TResult>(sql, paramss, commandType: CommandType.StoredProcedure);

            return serviceModel;
        }

        private int NextOffset(int pageSize, int pageNumber)
        {
            pageNumber += 1;

            return pageSize * (pageNumber - 1);
        }

        private static ExpandoObject CreateExpandoFromObject(object source)
        {
            var result = new ExpandoObject();
            if (source == null)
                return result;

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

        #endregion Methods
    }
}
