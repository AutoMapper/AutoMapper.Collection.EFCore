using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutoMapper.EntityFrameworkCore
{
    public interface IPersistence<TTo>
    {
        /// <summary>
        /// Insert Or Update the <see cref="T:System.Data.Entity.DbSet`1"/> with <paramref name="from"/>
        /// </summary>
        /// <remarks>Uses <see cref="AutoMapper.EquivalencyExpression.EquivalentExpressions.GenerateEquality"/>> to find equality between Source and From Types to determine if insert or update</remarks>
        /// <typeparam name="TFrom">Source Type mapping from</typeparam>
        /// <param name="from">Object to update to <see cref="T:System.Data.Entity.DbSet`1"/></param>
        /// <returns>The updated or inserted entity</returns>
        TTo InsertOrUpdate<TFrom>(TFrom from) where TFrom : class;

        /// <summary>
        /// Insert Or Update the <see cref="T:System.Data.Entity.DbSet`1"/> with <paramref name="from"/> asynchronously
        /// </summary>
        /// <remarks>Uses <see cref="AutoMapper.EquivalencyExpression.EquivalentExpressions.GenerateEquality"/>> to find equality between Source and From Types to determine if insert or update</remarks>
        /// <typeparam name="TFrom">Source Type mapping from</typeparam>
        /// <param name="from">Object to update to <see cref="T:System.Data.Entity.DbSet`1"/></param>
        /// <param name="cancellationToken">A cancellation token to observe the task.</param>
        /// <returns>A task containing the updated or inserted entity</returns>
        Task<TTo> InsertOrUpdateAsync<TFrom>(TFrom from, CancellationToken cancellationToken = default) where TFrom : class;

        /// <summary>
        /// Insert Or Update the <see cref="T:System.Data.Entity.DbSet`1"/> with <paramref name="from"/>
        /// </summary>
        /// <remarks>Uses <see cref="AutoMapper.EquivalencyExpression.EquivalentExpressions.GenerateEquality"/>> to find equality between Source and From Types to determine if insert or update</remarks>
        /// <param name="type">Source Type mapping from</param>
        /// <param name="from">Object to update to <see cref="T:System.Data.Entity.DbSet`1"/></param>
        /// <returns>The updated or inserted entity</returns>
        TTo InsertOrUpdate(Type type, object from);

        /// <summary>
        /// Insert Or Update the <see cref="T:System.Data.Entity.DbSet`1"/> with <paramref name="from"/> asynchronously
        /// </summary>
        /// <remarks>Uses <see cref="AutoMapper.EquivalencyExpression.EquivalentExpressions.GenerateEquality"/>> to find equality between Source and From Types to determine if insert or update</remarks>
        /// <param name="type">Source Type mapping from</param>
        /// <param name="from">Object to update to <see cref="T:System.Data.Entity.DbSet`1"/></param>
        /// <param name="cancellationToken">A cancellation token to observe the task.</param>
        /// <returns>A task containing the updated or inserted entity</returns>
        Task<TTo> InsertOrUpdateAsync(Type type, object from, CancellationToken cancellationToken = default);

        /// <summary>
        /// Remove from <see cref="T:System.Data.Entity.DbSet`1"/> with <paramref name="from"/>
        /// </summary>
        /// <remarks>Uses <see cref="AutoMapper.EquivalencyExpression.EquivalentExpressions.GenerateEquality"/>> to find equality between Source and From Types to determine if insert or update</remarks>
        /// <typeparam name="TFrom">Source Type mapping from</typeparam>
        /// <param name="from">Object to remove that is Equivalent in <see cref="T:System.Data.Entity.DbSet`1"/></param>
        void Remove<TFrom>(TFrom from) where TFrom : class;

        /// <summary>
        /// Remove from <see cref="T:System.Data.Entity.DbSet`1"/> with <paramref name="from"/> asynchronously
        /// </summary>
        /// <remarks>Uses <see cref="AutoMapper.EquivalencyExpression.EquivalentExpressions.GenerateEquality"/>> to find equality between Source and From Types to determine if insert or update</remarks>
        /// <typeparam name="TFrom">Source Type mapping from</typeparam>
        /// <param name="from">Object to remove that is Equivalent in <see cref="T:System.Data.Entity.DbSet`1"/></param>
        /// <param name="cancellationToken">A cancellation token to observe the task.</param>
        /// <returns>A task object indicating the status of the asynchronous operation</returns>
        Task RemoveAsync<TFrom>(TFrom from, CancellationToken cancellationToken = default) where TFrom : class;

    }
}