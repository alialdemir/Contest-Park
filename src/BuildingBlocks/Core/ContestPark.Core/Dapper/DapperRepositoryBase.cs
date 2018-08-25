using ContestPark.Core.Interfaces;
using DapperExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Core.Dapper
{
    public class DapperRepositoryBase<TEntity> : DatabaseConnection, IRepository<TEntity> where TEntity : class, IEntity, new()
    {
        #region Constructor

        public DapperRepositoryBase(ISettingsBase settingsBase) : base(settingsBase)
        {
        }

        public DapperRepositoryBase(string connectionString) : base(connectionString)
        {
        }

        #endregion Constructor

        #region CRUD operations

        /// <summary>
        /// Silme işlemi
        /// </summary>
        /// <param name="id">Primary id</param>
        public bool Delete(int id)
        {
            var entity = Connection.Get<TEntity>(id);
            return Connection.Delete(entity);
        }

        /// <summary>
        /// Entity ekleme
        /// </summary>
        /// <param name="entity">Entity modeli</param>
        public dynamic Insert(TEntity entity)
        {
            return Connection.Insert(entity);
        }

        /// <summary>
        /// Multiple entity ekleme
        /// </summary>
        /// <param name="entity">Entity modeli</param>
        public async Task InsertAsync(IEnumerable<TEntity> entities)
        {
            await Connection.InsertAsync(entities);
        }

        /// <summary>
        /// Entity güncelleme
        /// </summary>
        /// <param name="entity">Entity modeli</param>
        public bool Update(TEntity entity)
        {
            return Connection.Update(entity);
        }

        /// <summary>
        /// id'ye göre entity verir
        /// </summary>
        /// <param name="id">1</param>
        /// <returns>Entity modeli</returns>
        public int GetCount()
        {
            return Connection.GetList<TEntity>().Count();
        }

        #endregion CRUD operations
    }
}