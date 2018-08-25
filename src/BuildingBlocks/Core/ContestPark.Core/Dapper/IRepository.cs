using ContestPark.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Core.Dapper
{
    /// <summary>
    /// Repository arayüzü
    /// </summary>
    /// <typeparam name="T">Generic entity</typeparam>
    public interface IRepository<TEntity> : IDisposable where TEntity : class, IEntity, new()
    {
        dynamic Insert(TEntity entity);

        Task InsertAsync(IEnumerable<TEntity> entities);

        bool Update(TEntity entity);

        bool Delete(int id);

        int GetCount();
    }
}