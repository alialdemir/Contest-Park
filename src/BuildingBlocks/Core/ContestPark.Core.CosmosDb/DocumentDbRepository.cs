using ContestPark.Core.Database.Interfaces;
using Cosmonaut;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ContestPark.Core.CosmosDb
{
    public class DocumentDbRepository<TDocument> : IRepository<TDocument> where TDocument : class, IEntity, new()
    {
        #region Private variables

        private readonly ICosmosStore<TDocument> _cosmosStore;
        private readonly ILogger<DocumentDbRepository<TDocument>> _logger;

        #endregion Private variables

        #region Constructor

        public DocumentDbRepository(ICosmosStore<TDocument> cosmosStore,
                                    ILogger<DocumentDbRepository<TDocument>> logger)
        {
            _cosmosStore = cosmosStore ?? throw new ArgumentNullException(nameof(cosmosStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Toplam kayıt sayısını verir
        /// </summary>
        /// <returns>Total docs count</returns>
        public async Task<int> CountAsync()
        {
            return await _cosmosStore.QuerySingleAsync<int>("SELECT VALUE COUNT(c.id) FROM c");
        }

        /// <summary>
        /// Document sil
        /// </summary>
        /// <param name="id">document id</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> RemoveAsync(dynamic id)
        {
            try
            {
                string idToString = (string)id;

                var response = await _cosmosStore.RemoveAsync(x => ((string)x.Id) == idToString);

                return response.IsSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Document silerken hata oluştu. Collection Name: {_cosmosStore.CollectionName} id: {id}", ex);
                return false;
            }
        }

        /// <summary>
        /// Id ait kayıt döndürür yoksa null döner
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Document object or null</returns>
        public TDocument FindById(dynamic id)
        {
            try
            {
                return _cosmosStore.FindAsync(id.ToString()).Result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetById hata oluştu. Collection Name: {_cosmosStore.CollectionName} id: {id}", ex);

                return null;
            }
        }

        /// <summary>
        /// Parametreden gelen idlere ait tüm kayıtlar
        /// </summary>
        /// <param name="ids">id</param>
        /// <returns>Idlere ait tüm kayıtlar</returns>
        public IEnumerable<TDocument> FindByIds(IEnumerable<dynamic> ids)
        {
            try
            {
                return _cosmosStore.QueryMultipleAsync("SELECT * FROM c WHERE ARRAY_CONTAINS(@ids, c.id, true)", new { ids }).Result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetById hata oluştu. Collection Name: {_cosmosStore.CollectionName}", ex);

                return null;
            }
        }

        /// <summary>
        /// Document ekler
        /// </summary>
        /// <param name="document">Eklenen document</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> AddAsync(TDocument document)
        {
            try
            {
                document.CreatedDate = DateTime.Now;

                var response = await _cosmosStore.AddAsync(document);

                return response.IsSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Collection eklenirken hata oluştu. Collection Name: {_cosmosStore.CollectionName}", ex);

                return false;
            }
        }

        /// <summary>
        /// Çoklu document ekleme
        /// </summary>
        /// <param name="entities">Güncellenmek istenen dökümentler</param>
        /// <returns>Hepsi başarılı ise true değilse false</returns>
        public async Task<bool> AddRangeAsync(IEnumerable<TDocument> documents)
        {
            try
            {
                var response = await _cosmosStore.AddRangeAsync(documents);

                return response.FailedEntities.Count == 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Multiple collection eklenirken hata oluştu. Collection Name: {_cosmosStore.CollectionName}", ex);

                return false;
            }
        }

        /// <summary>
        /// Document güncelle
        /// </summary>
        /// <param name="document">Güncellenmek istenen document</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> UpdateAsync(TDocument document)
        {
            try
            {
                document.ModifiedDate = DateTime.Now;
                var result = await _cosmosStore.UpdateAsync(document);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Document güncellenirken hata oluştu. Collection Name: {_cosmosStore.CollectionName} id: {document.Id}", ex);

                return false;
            }
        }

        /// <summary>
        /// Çoklu document güncelleme
        /// </summary>
        /// <param name="entities">Güncellenmek istenen dökümentler</param>
        /// <returns>Hepsi başarılı ise true değilse false</returns>
        public async Task<bool> UpdateRangeAsync(IEnumerable<TDocument> entities)
        {
            try
            {
                entities.ToList().ForEach(x => x.ModifiedDate = DateTime.Now);

                var result = await _cosmosStore.UpdateRangeAsync(entities);

                return result.FailedEntities.Count == 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Documents güncelleme hata oluştu. Collection Name: {_cosmosStore.CollectionName}", ex);

                return false;
            }
        }

        public TDocument QuerySingle(string sql, object parameters = null)
        {
            return _cosmosStore.QuerySingleAsync(sql, parameters).Result;
        }

        public T QuerySingle<T>(string sql, object parameters = null)
        {
            return _cosmosStore.QuerySingleAsync<T>(sql, parameters).Result;
        }

        public IEnumerable<T> QueryMultiple<T>(string sql, object parameters = null)
        {
            return _cosmosStore.QueryMultipleAsync<T>(sql, parameters).Result;
        }

        public async Task<bool> RemoveAsync(Expression<Func<TDocument, bool>> predicate)
        {
            try
            {
                var result = await _cosmosStore.RemoveAsync(predicate);

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Documents silme hatası oluştu. Collection Name: {_cosmosStore.CollectionName}", ex);

                return false;
            }
        }

        #endregion Methods
    }
}
