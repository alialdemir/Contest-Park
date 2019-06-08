﻿using ContestPark.Core.CosmosDb.Infrastructure;
using ContestPark.Core.CosmosDb.Interfaces;
using ContestPark.Core.CosmosDb.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Core.CosmosDb
{
    public class DocumentDbRepository<TDocument> : IDocumentDbRepository<TDocument> where TDocument : class, IDocument, new()
    {
        #region Private variables

        private AsyncLazy<DocumentCollection> _collection;
        private readonly AsyncLazy<Database> _database;

        private readonly string _collectionName;
        private readonly string _databaseId;
        private readonly ILogger<DocumentDbRepository<TDocument>> _logger;

        #endregion Private variables

        #region Constructor

        public DocumentDbRepository(IDocumentDbInitializer client,
                                    DocumentDbConnection dbConnection,
                                    ILogger<DocumentDbRepository<TDocument>> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
            _databaseId = dbConnection.CosmosDbDatabaseId ?? throw new ArgumentNullException(nameof(dbConnection.CosmosDbDatabaseId));
            Client = client.GetClient(dbConnection);

            _collectionName = typeof(TDocument).Name;

            _database = new AsyncLazy<Database>(async () => await Client.CreateDatabaseIfNotExistsAsync(new Database { Id = dbConnection.CosmosDbDatabaseId }));// Database yoksa oluştur
            _collection = new AsyncLazy<DocumentCollection>(async () =>// Collection yoksa oluştur
            {
                return await Client.CreateDocumentCollectionIfNotExistsAsync(
                   UriFactory.CreateDatabaseUri(dbConnection.CosmosDbDatabaseId),
                   new DocumentCollection { Id = _collectionName });
            });

            CollectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionName);
        }

        #endregion Constructor

        #region Properties

        public IDocumentClient Client { get; private set; }
        public Uri CollectionUri { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Database ve collection data önce oluşturulmamış ise oluşturuyoruz
        /// </summary>
        public async Task Init()
        {
            string dbSelfLink = (await _database).SelfLink;// database oluşturdum
            string collectionSelfLink = (await _collection).SelfLink;
        }

        /// <summary>
        /// Toplam kayıt sayısını verir
        /// </summary>
        /// <returns></returns>
        public async Task<int> CountAsync()
        {
            return Client.CreateDocumentQuery<TDocument>((await _collection).SelfLink).Count();
        }

        /// <summary>
        /// Document sil
        /// </summary>
        /// <param name="id">document id</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                //Document document = Client.CreateDocumentQuery(
                //    CollectionUri,
                //    new SqlQuerySpec(
                //        $"SELECT * FROM {_collectionName} d WHERE d.id = @id",
                //        new SqlParameterCollection(new[] { new SqlParameter { Name = "@id", Value = id } })
                //        )).FirstOrDefault();

                //if (document != null)
                //{
                //    await Client.DeleteDocumentAsync(document.SelfLink);

                //    return true;
                //}

                await Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionName, id));

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Document silerken hata oluştu. Collection Name: {_collectionName} id: {id}", ex);
                return false;
            }
        }

        /// <summary>
        /// Id ait kayıt döndürür yoksa null döner
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Document object or null</returns>
        public TDocument GetById(string id)
        {
            try
            {
                return Query<TDocument>(new SqlQuerySpec
                {
                    QueryText = $"SELECT TOP 1 * FROM c WHERE (c.id = @id)",
                    Parameters = new SqlParameterCollection()
                    {
                          new SqlParameter("@id", id)
                    }
                }, new FeedOptions
                {
                    MaxItemCount = 1
                }).ToList().First();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetById hata oluştu. Collection Name: {_collectionName} id: {id}", ex);

                return null;
            }
        }

        /// <summary>
        /// Document ekler
        /// </summary>
        /// <param name="document">Eklenen document</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> InsertAsync(TDocument document)
        {
            try
            {
                document.CreatedDate = DateTime.Now;

                await Client.CreateDocumentAsync(CollectionUri, document);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Collection eklenirken hata oluştu. Collection Name: {_collectionName}", ex);
                return false;
            }
        }

        /// <summary>
        /// Liste olarak document ekler
        /// </summary>
        /// <param name="documents">Document collections</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> InsertAsync(IEnumerable<TDocument> documents)
        {
            try
            {
                foreach (TDocument document in documents)
                {
                    await InsertAsync(document);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Multiple collection eklenirken hata oluştu. ", ex);

                return false;
            }
        }

        /// <summary>
        /// Document güncelle
        /// </summary>
        /// <param name="document">güncellenen document</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> UpdateAsync(TDocument document)
        {
            try
            {
                document.ModifiedDate = DateTime.Now;
                var result = await Client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionName, document.Id), document);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Document güncellenirken hata oluştu. id: {document.Id}", ex);

                return false;
            }
        }

        /// <summary>
        /// Collections üzerinde query yapmaya yarar
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlExpression"></param>
        /// <param name="feedOptions"></param>
        /// <returns></returns>
        public IQueryable<T> Query<T>(string sqlExpression, FeedOptions feedOptions = null)
        {
            return Query<T>(new SqlQuerySpec
            {
                QueryText = sqlExpression,
            }, feedOptions);
        }

        /// <summary>
        /// Sql query ile sorgu yapar
        /// </summary>
        /// <typeparam name="T">Geriye dönen model</typeparam>
        /// <param name="querySpec">Sql query</param>
        /// <param name="feedOptions">Options</param>
        /// <returns></returns>
        public IQueryable<T> Query<T>(SqlQuerySpec querySpec, FeedOptions feedOptions = null)
        {
            return Client.CreateDocumentQuery<T>(CollectionUri, querySpec, feedOptions);
        }

        /// <summary>
        /// Parametreden gelen idlere ait tüm kayıtlar
        /// </summary>
        /// <param name="ids">id</param>
        /// <returns>Idlere ait tüm kayıtlar</returns>
        public IEnumerable<TDocument> GetByIds(params string[] ids)
        {
            try
            {
                string parameters = string.Join(", ", ids.Select(s => $"'{s}'"));
                return Query<TDocument>(new SqlQuerySpec
                {
                    QueryText = $"SELECT * FROM c WHERE c.id IN ({parameters})",
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetById hata oluştu. Collection Name: {_collectionName} ids: {ids}", ex);

                return null;
            }
        }

        #endregion Methods
    }
}