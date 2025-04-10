using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NuvTools.Common.ResultWrapper;
using NuvTools.Data.EntityFrameworkCore.Context;
using NuvTools.Data.EntityFrameworkCore.Extensions;
using System.Linq.Expressions;

namespace NuvTools.Security.Identity.EntityFrameworkCore;

public class IdentityIntDbContext : IdentityDbContextBase<IdentityUser<int>, IdentityRole<int>, int>;

public class IdentityGuidDbContext : IdentityDbContextBase<IdentityUser<Guid>, IdentityRole<Guid>, Guid>;

public class IdentityIntDbContextBase<TUser, TRole> : IdentityDbContextBase<TUser, TRole, int>
                                                                where TUser : IdentityUser<int>
                                                                where TRole : IdentityRole<int>;

public class IdentityGuidDbContextBase<TUser, TRole> : IdentityDbContextBase<TUser, TRole, Guid>
                                                                where TUser : IdentityUser<Guid>
                                                                where TRole : IdentityRole<Guid>;

public abstract class IdentityDbContextBase<TUser, TRole, TIdentityKey> : IdentityDbContext<TUser, TRole, TIdentityKey>, IDbContextCommands, IDbContextWithListCommands
                                                        where TUser : IdentityUser<TIdentityKey>
                                                        where TRole : IdentityRole<TIdentityKey>
                                                        where TIdentityKey : IEquatable<TIdentityKey>
{

    protected IdentityDbContextBase()
    {
    }

    protected IdentityDbContextBase(DbContextOptions options) : base(options)
    {
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Database.BeginTransactionAsync(cancellationToken);
    }

    public Task ExecuteWithStrategyAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
    {
        return DbContextExtensions.ExecuteWithStrategyAsync(this, action, cancellationToken);
    }

    public Task<T> ExecuteWithStrategyAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken = default)
    {
        return DbContextExtensions.ExecuteWithStrategyAsync(this, action, cancellationToken);
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Database.CommitTransactionAsync(cancellationToken);
    }
    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Database.RollbackTransactionAsync(cancellationToken);
    }

    public IDbContextTransaction? CurrentTransaction { get { return Database.CurrentTransaction; } }

    public Task<IResult<TKey>> AddAndSaveAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
    {
        return DbContextExtensions.AddAndSaveAsync<TEntity, TKey>(this, entity, cancellationToken);
    }

    public Task<IResult> UpdateAndSaveAsync<TEntity>(TEntity entity, params object[] keyValues) where TEntity : class
    {
        return DbContextExtensions.UpdateAndSaveAsync(this, entity, keyValues);
    }

    public Task<IResult> UpdateAndSaveAsync<TEntity>(TEntity entity, object[] keyValues, CancellationToken cancellationToken = default) where TEntity : class
    {
        return DbContextExtensions.UpdateAndSaveAsync(this, entity, keyValues, cancellationToken);
    }

    public Task<IResult> RemoveAndSaveAsync<TEntity>(params object[] keyValues) where TEntity : class
    {
        return DbContextExtensions.RemoveAndSaveAsync<TEntity>(this, keyValues);
    }

    public Task<IResult> RemoveAndSaveAsync<TEntity>(object[] keyValues, CancellationToken cancellationToken = default) where TEntity : class
    {
        return DbContextExtensions.RemoveAndSaveAsync<TEntity>(this, keyValues, cancellationToken);
    }

    public Task<IResult<object[]>> AddAndSaveWithCompositeKeyAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
    {
        return DbContextExtensions.AddAndSaveWithCompositeKeyAsync(this, entity, cancellationToken);
    }

    public Task<IResult> SyncFromListAsync<TEntity, TKey>(IEnumerable<TEntity> entities, Func<TEntity, TKey> keySelector,
                                                            Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
        where TEntity : class
        where TKey : notnull
    {
        return DbContextWithListExtensions.SyncFromListAsync(this, entities, keySelector, filter, cancellationToken);
    }

    public Task<IResult> AddOrUpdateFromListAsync<TEntity, TKey>(IEnumerable<TEntity> entities, Func<TEntity, TKey> keySelector,
                                                            Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
        where TEntity : class
        where TKey : notnull
    {
        return DbContextWithListExtensions.AddOrUpdateFromListAsync(this, entities, keySelector, filter, cancellationToken);
    }

    public Task<IResult> AddOrRemoveFromListAsync<TEntity, TKey>(IEnumerable<TEntity> entities, Func<TEntity, TKey> keySelector,
                                                            Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
        where TEntity : class
        where TKey : notnull
    {
        return DbContextWithListExtensions.AddOrRemoveFromListAsync(this, entities, keySelector, filter, cancellationToken);
    }

}
