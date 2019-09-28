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
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ContestPark.Core.Dapper
{
    public class DapperRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity, new()
    {
        #region Private variables

        private readonly IDatabaseConnection _databaseConnection;

        private readonly ILogger<DapperRepository<TEntity>> _logger;

        #endregion Private variables

        #region Constructor

        public DapperRepository(IDatabaseConnection databaseConnection,
                                ILogger<DapperRepository<TEntity>> logger)
        {
            _databaseConnection = databaseConnection;

            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.MySQL);

            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
        }

        public string TableName
        {
            get
            {
                return typeof(TEntity).Name;
            }
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Entity ekler
        /// </summary>
        /// <param name="entity">Eklenen entity</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<int?> AddAsync(TEntity entity)
        {
            try
            {
                return await _databaseConnection.Connection.InsertAsync<TEntity>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Collection eklenirken hata oluştu. table Name: {TableName}", ex.Message);

                return null;
            }
            finally
            {
                ((Disposable)_databaseConnection)?.DisposeCore();
            }
        }

        /// <summary>
        /// Entity ekler
        /// </summary>
        /// <param name="entity">Eklenen entity</param>
        /// <returns>Eklenen kayıt id</returns>
        public async Task<int?> AddAndGetIdAsync(TEntity entity)
        {
            try
            {
                return await _databaseConnection.Connection.InsertAsync<TEntity>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Collection eklenirken hata oluştu. table Name: {TableName}", ex.Message);

                return 0;
            }
            finally
            {
                ((Disposable)_databaseConnection)?.DisposeCore();
            }
        }

        /// <summary>
        /// Çoklu entity ekleme
        /// </summary>
        /// <param name="entities">Güncellenmek istenen entities</param>
        /// <returns>Hepsi başarılı ise true değilse false</returns>
        public async Task<bool> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            try
            {
                foreach (var entity in entities)
                {
                    await _databaseConnection.Connection.InsertAsync<TEntity>(entity);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Multiple collection eklenirken hata oluştu. table Name: {TableName}", ex.Message);

                return false;
            }
            finally
            {
                ((Disposable)_databaseConnection)?.DisposeCore();
            }
        }

        public async Task<bool> AddRangeAsync<Key>(IEnumerable<TEntity> entities)
        {
            try
            {
                foreach (var entity in entities)
                {
                    Key id = await _databaseConnection.Connection.InsertAsync<Key, TEntity>(entity);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Multiple collection eklenirken hata oluştu. table Name: {TableName}", ex.Message);

                return false;
            }
            finally
            {
                ((Disposable)_databaseConnection)?.DisposeCore();
            }
        }

        /// <summary>
        /// Toplam kayıt sayısını verir
        /// </summary>
        /// <returns>Total docs count</returns>
        public Task<int> CountAsync()
        {
            var result = _databaseConnection.Connection.RecordCountAsync<TEntity>();

            ((Disposable)_databaseConnection)?.DisposeCore();

            return result;
        }

        /// <summary>
        /// Id ait kayıt döndürür yoksa null döner
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Document object or null</returns>
        public TEntity FindById(dynamic id)
        {
            try
            {
                return _databaseConnection.Connection.Get<TEntity>((object)id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetById hata oluştu. table Name: {TableName} id: {id}", ex.Message);

                return null;
            }
            finally
            {
                ((Disposable)_databaseConnection)?.DisposeCore();
            }
        }

        /// <summary>
        /// Parametreden gelen idlere ait tüm kayıtlar
        /// </summary>
        /// <param name="ids">id</param>
        /// <returns>Idlere ait tüm kayıtlar</returns>
        public IEnumerable<TEntity> FindByIds(IEnumerable<dynamic> ids)
        {
            try
            {
                return _databaseConnection.Connection.GetList<TEntity>(ids);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetById hata oluştu. table Name: {TableName}", ex.Message);

                return null;
            }
            finally
            {
                ((Disposable)_databaseConnection)?.DisposeCore();
            }
        }

        /// <summary>
        /// Entity sil
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> RemoveAsync(object id)
        {
            try
            {
                string idToString = (string)id;

                var entity = FindById(id);

                int recordsAffected = await _databaseConnection.Connection.DeleteAsync<TEntity>(entity);

                return recordsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Document silerken hata oluştu. Table Name: {TableName} id: {id}", ex.Message);

                return false;
            }
            finally
            {
                ((Disposable)_databaseConnection)?.DisposeCore();
            }
        }

        /// <summary>
        /// ilgili kaydı sil
        /// </summary>
        /// <param name="predicate">Where expression</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> RemoveAsync(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                int recordsAffected = await _databaseConnection.Connection.DeleteAsync<TEntity>(predicate);

                return recordsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Documents silme hatası oluştu. Table Name: {TableName}", ex.Message);

                return false;
            }
            finally
            {
                ((Disposable)_databaseConnection)?.DisposeCore();
            }
        }

        /// <summary>
        /// Document güncelle
        /// </summary>
        /// <param name="document">Güncellenmek istenen document</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> UpdateAsync(TEntity entity)
        {
            try
            {
                int recordsAffected = await _databaseConnection.Connection.UpdateAsync<TEntity>(entity);

                return recordsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Document güncellenirken hata oluştu. Collection Name: {TableName} ", ex.Message);

                return false;
            }
            finally
            {
                ((Disposable)_databaseConnection)?.DisposeCore();
            }
        }

        /// <summary>
        /// Çoklu entity güncelleme
        /// </summary>
        /// <param name="entities">Güncellenmek istenen dökümentler</param>
        /// <returns>Hepsi başarılı ise true değilse false</returns>
        public async Task<bool> UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            try
            {
                int recordsAffected = 0;

                foreach (var item in entities)
                {
                    recordsAffected += await _databaseConnection.Connection.UpdateAsync<TEntity>(item);
                }

                return recordsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Documents güncelleme hata oluştu. Collection Name: {TableName}", ex.Message);

                return false;
            }
            finally
            {
                ((Disposable)_databaseConnection)?.DisposeCore();
            }
        }

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
                _logger.LogError($"Documents güncelleme hata oluştu. Collection Name: {TableName}", ex.Message);
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
        /// <typeparam name="TResult"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <param name="pagingModel"></param>
        /// <returns></returns>
        public ServiceModel<TResult> ToServiceModel<TResult>(string sql, object parameters = null, CommandType? commandType = null, PagingModel pagingModel = null)
        {
            dynamic paramss = CreateExpandoFromObject(parameters);
            paramss.PageSize = pagingModel.PageSize;
            paramss.Offset = pagingModel.Offset;
            paramss.Offset2 = NextOffset(pagingModel.PageSize, pagingModel.PageNumber);

            string query1 = sql + " LIMIT @Offset2, @PageSize; ";
            string query2 = sql + " LIMIT @Offset, @PageSize";

            long nextPageCount = QueryMultiple<object>(query1, (object)paramss).Count();

            ServiceModel<TResult> serviceModel = new ServiceModel<TResult>
            {
                PageSize = pagingModel.PageSize,
                PageNumber = pagingModel.PageNumber,
                HasNextPage = nextPageCount > 0
            };

            serviceModel.Items = QueryMultiple<TResult>(query2, (object)paramss);

            return serviceModel;
        }

        private int NextOffset(int pageSize, int pageNumber)
        {
            pageNumber = pageNumber + 1;

            return pageSize * (pageNumber - 1);
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

        #endregion Methods
    }
}
