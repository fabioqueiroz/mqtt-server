using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MQTT.CentralServer.Data.Access.Interfaces
{
    public interface IBaseRepository<TEntity> : IDisposable
    {
        /// <summary>
        /// Returns all items of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IQueryable<T> All<T>() where T : class;

        /// <summary>
        /// Returns all items with populated navigation properties of the specified type. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="include"></param>
        /// <returns></returns>
        IQueryable<T> AllIncluding<T>(params Expression<Func<T, object>>[] include) where T : class;

        /// <summary>
        /// Returns a single item of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="navigationProperties"></param>
        /// <returns></returns>
        T GetSingle<T>(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties) where T : class;

        /// <summary>
        /// Returns a single item of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        Task<T> GetSingleAsync<T>(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] include) where T : class;

        /// <summary>
        /// Persists the item to the datasource.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        T Add<T>(T entity) where T : class;

        /// <summary>
        /// Updates the item in the datasource.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        T Update<T>(T entity) where T : class;

        /// <summary>
        /// Deletes the item from the data source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Delete<T>(T entity) where T : class;

        /// <summary>
        /// Adds many items to the data source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        T[] AddMany<T>(params T[] entities) where T : class;

        /// <summary>
        /// Updates many items in the data source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        T[] UpdateMany<T>(params T[] entities) where T : class;

        /// <summary>
        /// Deletes many items from the data source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        int DeleteMany<T>(params T[] entities) where T : class;

        /// <summary>
        /// Persists the transaction to the data source.
        /// </summary>
        /// <returns></returns>
        int Commit();

        /// <summary>
        /// Persists the transaction to the data source.
        /// </summary>
        /// <returns></returns>
        Task<int> CommitAsync(CancellationToken cancellationToken = default);
    }
}
