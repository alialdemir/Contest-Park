using ContestPark.Core.Dapper.Interfaces;
using ContestPark.Core.Database.Interfaces;
using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
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
            var g = SimpleCRUD.GetDialect();
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
        public async Task<bool> AddAsync(TEntity entity)
        {
            try
            {
                int? id = await _databaseConnection.Connection.InsertAsync<TEntity>(entity);

                return id > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Collection eklenirken hata oluştu. table Name: {TableName}", ex);

                return false;
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
                int result = 0;
                foreach (var entity in entities)
                {
                    int? id = await _databaseConnection.Connection.InsertAsync<TEntity>(entity);
                    result += id ?? 0;
                }

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Multiple collection eklenirken hata oluştu. table Name: {TableName}", ex);

                return false;
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
                _logger.LogError($"Multiple collection eklenirken hata oluştu. table Name: {TableName}", ex);

                return false;
            }
        }

        /// <summary>
        /// Toplam kayıt sayısını verir
        /// </summary>
        /// <returns>Total docs count</returns>
        public Task<int> CountAsync()
        {
            return _databaseConnection.Connection.RecordCountAsync<TEntity>();
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
                _logger.LogError($"GetById hata oluştu. table Name: {TableName} id: {id}", ex);

                return null;
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
                _logger.LogError($"GetById hata oluştu. table Name: {TableName}", ex);

                return null;
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
                _logger.LogError($"Document silerken hata oluştu. Table Name: {TableName} id: {id}", ex);

                return false;
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
                _logger.LogError($"Documents silme hatası oluştu. Table Name: {TableName}", ex);

                return false;
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
                _logger.LogError($"Document güncellenirken hata oluştu. Collection Name: {TableName} ", ex);

                return false;
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
                _logger.LogError($"Documents güncelleme hata oluştu. Collection Name: {TableName}", ex);

                return false;
            }
        }

        public IEnumerable<T> QueryMultiple<T>(string sql, object parameters = null)
        {
            return _databaseConnection.Connection.QueryMultiple(sql, parameters).Read<T>();
        }

        public TEntity QuerySingle(string sql, object parameters = null)
        {
            return _databaseConnection.Connection.QuerySingle<TEntity>(sql, parameters);
        }

        public T QuerySingleOrDefault<T>(string sql, object parameters = null)
        {
            return _databaseConnection.Connection.QuerySingleOrDefault<T>(sql, parameters);
        }

        public T SpQuerySingleOrDefault<T>(string sql, object parameters = null)
        {
            return _databaseConnection.Connection.QuerySingleOrDefault<T>(sql, parameters, commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Sp çalıştırır
        /// </summary>
        /// <param name="sql">çalıştırılacak sql</param>
        /// <param name="parameters">Parametreleri</param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public async Task<bool> ExecuteAsync(string sql, object parameters = null)
        {
            try
            {
                int result = await _databaseConnection.Connection.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Documents güncelleme hata oluştu. Collection Name: {TableName}", ex);
                return false;
            }
        }

        #endregion Methods
    }
}
