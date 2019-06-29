using ContestPark.Core.Dapper.Interfaces;
using ContestPark.Core.Database.Interfaces;
using Dapper;
using DapperExtensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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

        #region MyRegion

        /// <summary>
        /// Entity ekler
        /// </summary>
        /// <param name="entity">Eklenen entity</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> AddAsync(TEntity entity)
        {
            try
            {
                entity.CreatedDate = DateTime.Now;

                int id = await _databaseConnection.Connection.InsertAsync(entity);

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
                await _databaseConnection.Connection.InsertAsync(entities);

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
            return _databaseConnection.Connection.CountAsync<TEntity>();
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

                return await _databaseConnection.Connection.DeleteAsync<TEntity>(entity);
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
                return await _databaseConnection.Connection.DeleteAsync(predicate);
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
                entity.ModifiedDate = DateTime.Now;
                return await _databaseConnection.Connection.UpdateAsync<TEntity>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Document güncellenirken hata oluştu. Collection Name: {TableName} id: {entity.Id}", ex);

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
                entities.ToList().ForEach(x => x.ModifiedDate = DateTime.Now);

                return await _databaseConnection.Connection.UpdateAsync(entities);
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

        public T QuerySingle<T>(string sql, object parameters = null)
        {
            return _databaseConnection.Connection.QuerySingle<T>(sql, parameters);
        }

        #endregion MyRegion
    }
}
