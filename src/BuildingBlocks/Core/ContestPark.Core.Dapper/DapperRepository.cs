using ContestPark.Core.Dapper.Abctract;
using ContestPark.Core.Dapper.Interfaces;
using ContestPark.Core.Database.Interfaces;
using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ContestPark.Core.Dapper
{
    public class DapperRepository<TEntity> : DapperQueryRepository, IRepository<TEntity> where TEntity : class, IEntity, new()
    {
        #region Constructor

        public DapperRepository(IDatabaseConnection databaseConnection,
                                ILogger<DapperRepository<TEntity>> logger) : base(databaseConnection, logger)
        {
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
                _logger.LogError(ex, $"Collection eklenirken hata oluştu. table Name: {TableName}");

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
                _logger.LogError(ex, $"Collection eklenirken hata oluştu. table Name: {TableName}");

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
                _logger.LogError(ex, $"Multiple collection eklenirken hata oluştu. table Name: {TableName}");

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
                _logger.LogError(ex, $"Multiple collection eklenirken hata oluştu. table Name: {TableName}");

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
                _logger.LogError(ex, $"GetById hata oluştu. table Name: {TableName} id: {id}");

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
                _logger.LogError(ex, $"GetById hata oluştu. table Name: {TableName}");

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
                _logger.LogError(ex, $"Document silerken hata oluştu. Table Name: {TableName} id: {id}");

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
                _logger.LogError(ex, $"Documents silme hatası oluştu. Table Name: {TableName}");

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
                _logger.LogError(ex, $"Document güncellenirken hata oluştu. Collection Name: {TableName} ");

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
                _logger.LogError(ex, $"Documents güncelleme hata oluştu. Collection Name: {TableName}");

                return false;
            }
            finally
            {
                ((Disposable)_databaseConnection)?.DisposeCore();
            }
        }

        #endregion Methods
    }
}
