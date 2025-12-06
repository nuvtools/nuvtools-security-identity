using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NuvTools.Common.ResultWrapper;
using NuvTools.Data.EntityFrameworkCore.Context;
using NuvTools.Data.EntityFrameworkCore.Extensions;
using System.Linq.Expressions;

namespace NuvTools.Security.Identity.EntityFrameworkCore;

/// <summary>
/// Provides a base implementation of <see cref="IdentityDbContext{TUser, TRole, TKey}"/> with enhanced
/// transaction support, execution strategies, and batch operation capabilities.
/// </summary>
/// <typeparam name="TUser">The type representing a user in the system, must inherit from <see cref="IdentityUser{TKey}"/>.</typeparam>
/// <typeparam name="TRole">The type representing a role in the system, must inherit from <see cref="IdentityRole{TKey}"/>.</typeparam>
/// <typeparam name="TIdentityKey">The type of the primary key for users and roles, must implement <see cref="IEquatable{T}"/>.</typeparam>
/// <remarks>
/// <para>
/// This abstract base class extends <see cref="IdentityDbContext{TUser, TRole, TKey}"/> and implements
/// <see cref="IDbContextCommands"/> and <see cref="IDbContextWithListCommands"/> to provide:
/// </para>
/// <list type="bullet">
/// <item><description>Transaction management with begin, commit, and rollback operations</description></item>
/// <item><description>Execution strategies for handling transient database failures</description></item>
/// <item><description>Convenient CRUD operations that automatically save changes and return <see cref="IResult"/> wrappers</description></item>
/// <item><description>Batch operations for synchronizing collections of entities</description></item>
/// <item><description>Support for composite key operations</description></item>
/// </list>
/// <para>
/// All operations use the Result pattern, returning <see cref="IResult"/> or <see cref="IResult{T}"/>
/// instead of throwing exceptions for business logic failures.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// public class ApplicationDbContext : IdentityDbContextBase&lt;ApplicationUser, ApplicationRole, Guid&gt;
/// {
///     public ApplicationDbContext(DbContextOptions&lt;ApplicationDbContext&gt; options)
///         : base(options)
///     {
///     }
///
///     protected override void OnModelCreating(ModelBuilder builder)
///     {
///         base.OnModelCreating(builder);
///         // Configure your entities here
///     }
/// }
/// </code>
/// </example>
public abstract class IdentityDbContextBase<TUser, TRole, TIdentityKey> : IdentityDbContext<TUser, TRole, TIdentityKey>, IDbContextCommands, IDbContextWithListCommands
                                                        where TUser : IdentityUser<TIdentityKey>
                                                        where TRole : IdentityRole<TIdentityKey>
                                                        where TIdentityKey : IEquatable<TIdentityKey>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityDbContextBase{TUser, TRole, TIdentityKey}"/> class.
    /// </summary>
    /// <remarks>
    /// This parameterless constructor is typically used for design-time tools and migrations.
    /// </remarks>
    protected IdentityDbContextBase()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityDbContextBase{TUser, TRole, TIdentityKey}"/> class
    /// with the specified options.
    /// </summary>
    /// <param name="options">The options to be used by the <see cref="DbContext"/>.</param>
    /// <remarks>
    /// This constructor is used for runtime configuration and dependency injection scenarios.
    /// </remarks>
    protected IdentityDbContextBase(DbContextOptions options) : base(options)
    {
    }

    /// <summary>
    /// Asynchronously begins a new database transaction.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an <see cref="IDbContextTransaction"/>
    /// that represents the started transaction.
    /// </returns>
    /// <remarks>
    /// Transactions allow multiple database operations to be executed as a single atomic unit.
    /// Use <see cref="CommitTransactionAsync"/> to commit the transaction or <see cref="RollbackTransactionAsync"/> to roll it back.
    /// </remarks>
    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Executes an action within the configured execution strategy to handle transient database failures.
    /// </summary>
    /// <param name="action">The action to execute that takes a <see cref="CancellationToken"/> and returns a <see cref="Task"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// The execution strategy automatically retries operations that fail due to transient errors,
    /// such as temporary network issues or database deadlocks, according to the configured retry policy.
    /// </remarks>
    public Task ExecuteWithStrategyAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
    {
        return DbContextExtensions.ExecuteWithStrategyAsync(this, action, cancellationToken);
    }

    /// <summary>
    /// Executes a function within the configured execution strategy to handle transient database failures.
    /// </summary>
    /// <typeparam name="T">The type of the result returned by the function.</typeparam>
    /// <param name="action">The function to execute that takes a <see cref="CancellationToken"/> and returns a <see cref="Task{T}"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the value returned by the function.
    /// </returns>
    /// <remarks>
    /// The execution strategy automatically retries operations that fail due to transient errors,
    /// such as temporary network issues or database deadlocks, according to the configured retry policy.
    /// </remarks>
    public Task<T> ExecuteWithStrategyAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken = default)
    {
        return DbContextExtensions.ExecuteWithStrategyAsync(this, action, cancellationToken);
    }

    /// <summary>
    /// Asynchronously commits all changes made in the current transaction to the database.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// This method should be called after <see cref="BeginTransactionAsync"/> to persist the changes.
    /// If the commit fails, the transaction remains active and can be rolled back.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown when there is no active transaction.</exception>
    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Database.CommitTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Asynchronously rolls back all changes made in the current transaction.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// This method discards all changes made within the current transaction and closes it.
    /// After rolling back, you need to call <see cref="BeginTransactionAsync"/> to start a new transaction.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown when there is no active transaction.</exception>
    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Database.RollbackTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the current active transaction, or <see langword="null"/> if no transaction is active.
    /// </summary>
    /// <value>
    /// An <see cref="IDbContextTransaction"/> representing the current transaction, or <see langword="null"/> if no transaction has been started.
    /// </value>
    /// <remarks>
    /// Use this property to check whether a transaction is currently active before performing transaction-specific operations.
    /// </remarks>
    public IDbContextTransaction? CurrentTransaction { get { return Database.CurrentTransaction; } }

    /// <summary>
    /// Asynchronously adds an entity to the database and saves the changes, returning the generated primary key.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to add. Must be a reference type.</typeparam>
    /// <typeparam name="TKey">The type of the primary key.</typeparam>
    /// <param name="entity">The entity instance to add to the database.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an <see cref="IResult{T}"/>
    /// with the generated primary key value if successful.
    /// </returns>
    /// <remarks>
    /// This method automatically calls <see cref="DbContext.SaveChangesAsync(CancellationToken)"/> after adding the entity.
    /// The result will contain the primary key value generated by the database (for identity columns or sequences).
    /// </remarks>
    public Task<IResult<TKey>> AddAndSaveAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
    {
        return DbContextExtensions.AddAndSaveAsync<TEntity, TKey>(this, entity, cancellationToken);
    }

    /// <summary>
    /// Asynchronously updates an existing entity in the database and saves the changes.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to update. Must be a reference type.</typeparam>
    /// <param name="entity">The entity instance with updated values.</param>
    /// <param name="keyValues">The primary key values used to locate the existing entity in the database.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an <see cref="IResult"/>
    /// indicating whether the update was successful.
    /// </returns>
    /// <remarks>
    /// This method finds the entity by its primary key, updates its values, and automatically saves the changes.
    /// If the entity is not found, the operation will fail and return an unsuccessful result.
    /// </remarks>
    public Task<IResult> UpdateAndSaveAsync<TEntity>(TEntity entity, params object[] keyValues) where TEntity : class
    {
        return DbContextExtensions.UpdateAndSaveAsync(this, entity, keyValues);
    }

    /// <summary>
    /// Asynchronously updates an existing entity in the database and saves the changes.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to update. Must be a reference type.</typeparam>
    /// <param name="entity">The entity instance with updated values.</param>
    /// <param name="keyValues">The primary key values used to locate the existing entity in the database.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an <see cref="IResult"/>
    /// indicating whether the update was successful.
    /// </returns>
    /// <remarks>
    /// This method finds the entity by its primary key, updates its values, and automatically saves the changes.
    /// If the entity is not found, the operation will fail and return an unsuccessful result.
    /// </remarks>
    public Task<IResult> UpdateAndSaveAsync<TEntity>(TEntity entity, object[] keyValues, CancellationToken cancellationToken = default) where TEntity : class
    {
        return DbContextExtensions.UpdateAndSaveAsync(this, entity, keyValues, cancellationToken);
    }

    /// <summary>
    /// Asynchronously removes an entity from the database by its primary key and saves the changes.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to remove. Must be a reference type.</typeparam>
    /// <param name="keyValues">The primary key values identifying the entity to remove.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an <see cref="IResult"/>
    /// indicating whether the removal was successful.
    /// </returns>
    /// <remarks>
    /// This method finds the entity by its primary key, removes it, and automatically saves the changes.
    /// If the entity is not found, the operation may still succeed (idempotent deletion).
    /// </remarks>
    public Task<IResult> RemoveAndSaveAsync<TEntity>(params object[] keyValues) where TEntity : class
    {
        return DbContextExtensions.RemoveAndSaveAsync<TEntity>(this, keyValues);
    }

    /// <summary>
    /// Asynchronously removes an entity from the database by its primary key and saves the changes.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to remove. Must be a reference type.</typeparam>
    /// <param name="keyValues">The primary key values identifying the entity to remove.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an <see cref="IResult"/>
    /// indicating whether the removal was successful.
    /// </returns>
    /// <remarks>
    /// This method finds the entity by its primary key, removes it, and automatically saves the changes.
    /// If the entity is not found, the operation may still succeed (idempotent deletion).
    /// </remarks>
    public Task<IResult> RemoveAndSaveAsync<TEntity>(object[] keyValues, CancellationToken cancellationToken = default) where TEntity : class
    {
        return DbContextExtensions.RemoveAndSaveAsync<TEntity>(this, keyValues, cancellationToken);
    }

    /// <summary>
    /// Asynchronously adds an entity with a composite key to the database and saves the changes,
    /// returning the composite key values.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to add. Must be a reference type.</typeparam>
    /// <param name="entity">The entity instance to add to the database.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an <see cref="IResult{T}"/>
    /// with an array of composite key values if successful.
    /// </returns>
    /// <remarks>
    /// This method is used for entities that have composite primary keys (multiple columns forming the primary key).
    /// The returned array contains the key values in the order they are defined in the entity configuration.
    /// </remarks>
    public Task<IResult<object[]>> AddAndSaveWithCompositeKeyAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
    {
        return DbContextExtensions.AddAndSaveWithCompositeKeyAsync(this, entity, cancellationToken);
    }

    /// <summary>
    /// Asynchronously synchronizes a collection of entities with the database by adding new entities,
    /// updating existing ones, and removing entities not present in the collection.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to synchronize. Must be a reference type.</typeparam>
    /// <typeparam name="TKey">The type of the key used to identify entities. Must be non-nullable.</typeparam>
    /// <param name="entities">The collection of entities to synchronize with the database.</param>
    /// <param name="keySelector">A function to extract the key from an entity for comparison.</param>
    /// <param name="filter">An optional filter expression to limit which database entities are considered for synchronization.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an <see cref="IResult"/>
    /// indicating whether the synchronization was successful.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method performs a full synchronization:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Entities in the collection but not in the database are added</description></item>
    /// <item><description>Entities in both the collection and database are updated with values from the collection</description></item>
    /// <item><description>Entities in the database (matching the filter) but not in the collection are removed</description></item>
    /// </list>
    /// <para>
    /// This is useful for maintaining master-detail relationships or synchronizing complete lists.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Synchronize user roles - adds new, updates existing, removes missing
    /// var userRoles = newRoleIds.Select(roleId => new IdentityUserRole&lt;Guid&gt; { UserId = userId, RoleId = roleId });
    /// await context.SyncFromListAsync(userRoles, ur => ur.RoleId, ur => ur.UserId == userId);
    /// </code>
    /// </example>
    public Task<IResult> SyncFromListAsync<TEntity, TKey>(IEnumerable<TEntity> entities, Func<TEntity, TKey> keySelector,
                                                            Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
        where TEntity : class
        where TKey : notnull
    {
        return DbContextWithListExtensions.SyncFromListAsync(this, entities, keySelector, filter, cancellationToken);
    }

    /// <summary>
    /// Asynchronously adds new entities from a collection or updates existing ones, without removing any entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to add or update. Must be a reference type.</typeparam>
    /// <typeparam name="TKey">The type of the key used to identify entities. Must be non-nullable.</typeparam>
    /// <param name="entities">The collection of entities to add or update in the database.</param>
    /// <param name="keySelector">A function to extract the key from an entity for comparison.</param>
    /// <param name="filter">An optional filter expression to limit which database entities are considered for matching.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an <see cref="IResult"/>
    /// indicating whether the operation was successful.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method performs an upsert operation:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Entities not found in the database are added</description></item>
    /// <item><description>Entities found in the database are updated with values from the collection</description></item>
    /// <item><description>No entities are removed from the database</description></item>
    /// </list>
    /// <para>
    /// This is useful when you want to ensure certain entities exist and are up-to-date without deleting others.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Add or update permissions without removing existing ones
    /// var permissions = new[] { "Read", "Write", "Execute" }.Select(p => new Permission { Name = p });
    /// await context.AddOrUpdateFromListAsync(permissions, p => p.Name);
    /// </code>
    /// </example>
    public Task<IResult> AddOrUpdateFromListAsync<TEntity, TKey>(IEnumerable<TEntity> entities, Func<TEntity, TKey> keySelector,
                                                            Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
        where TEntity : class
        where TKey : notnull
    {
        return DbContextWithListExtensions.AddOrUpdateFromListAsync(this, entities, keySelector, filter, cancellationToken);
    }

    /// <summary>
    /// Asynchronously adds new entities from a collection or removes existing ones that are not in the collection,
    /// without updating existing entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to add or remove. Must be a reference type.</typeparam>
    /// <typeparam name="TKey">The type of the key used to identify entities. Must be non-nullable.</typeparam>
    /// <param name="entities">The collection of entities to ensure exist in the database.</param>
    /// <param name="keySelector">A function to extract the key from an entity for comparison.</param>
    /// <param name="filter">An optional filter expression to limit which database entities are considered for removal.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an <see cref="IResult"/>
    /// indicating whether the operation was successful.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method performs selective add/remove operations:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Entities in the collection but not in the database are added</description></item>
    /// <item><description>Entities in the database (matching the filter) but not in the collection are removed</description></item>
    /// <item><description>Existing entities that are in both the collection and database are left unchanged (not updated)</description></item>
    /// </list>
    /// <para>
    /// This is useful when you only care about the presence of entities, not their current values.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Ensure specific tags exist for a post without updating existing tag data
    /// var tags = new[] { "C#", "ASP.NET", "EF Core" }.Select(t => new Tag { Name = t, PostId = postId });
    /// await context.AddOrRemoveFromListAsync(tags, t => t.Name, t => t.PostId == postId);
    /// </code>
    /// </example>
    public Task<IResult> AddOrRemoveFromListAsync<TEntity, TKey>(IEnumerable<TEntity> entities, Func<TEntity, TKey> keySelector,
                                                            Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
        where TEntity : class
        where TKey : notnull
    {
        return DbContextWithListExtensions.AddOrRemoveFromListAsync(this, entities, keySelector, filter, cancellationToken);
    }

}
