using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace NuvTools.Security.Identity.AspNetCore.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="RoleManager{TRole}"/> class to simplify the management of claims,
/// particularly permission-based claims, associated with roles.
/// </summary>
public static class RoleManagerExtensions
{
    /// <summary>
    /// Adds multiple claims to a role.
    /// </summary>
    /// <typeparam name="TRole">The type representing the application role.</typeparam>
    /// <param name="roleManager">The <see cref="RoleManager{TRole}"/> instance used to manage roles.</param>
    /// <param name="role">The role to which claims should be added.</param>
    /// <param name="claims">A collection of claims to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="roleManager"/>, <paramref name="role"/>, or <paramref name="claims"/> is null.</exception>
    public static async Task AddClaimsAsync<TRole>(
        this RoleManager<TRole> roleManager,
        TRole role,
        IEnumerable<Claim> claims)
        where TRole : class
    {
        ArgumentNullException.ThrowIfNull(roleManager);
        ArgumentNullException.ThrowIfNull(role);
        ArgumentNullException.ThrowIfNull(claims);

        foreach (var claim in claims)
        {
            await roleManager.AddClaimAsync(role, claim);
        }
    }

    /// <summary>
    /// Adds a single claim to a role if it does not already exist.
    /// </summary>
    /// <typeparam name="TRole">The type representing the application role.</typeparam>
    /// <param name="roleManager">The <see cref="RoleManager{TRole}"/> instance used to manage roles.</param>
    /// <param name="role">The role to which the claim should be added.</param>
    /// <param name="type">The claim type.</param>
    /// <param name="value">The claim value.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="roleManager"/>, <paramref name="role"/>, <paramref name="type"/>, or <paramref name="value"/> is null or empty.</exception>
    public static async Task AddClaimAsync<TRole>(
        this RoleManager<TRole> roleManager,
        TRole role,
        string type,
        string value)
        where TRole : class
    {
        ArgumentNullException.ThrowIfNull(roleManager);
        ArgumentNullException.ThrowIfNull(role);

        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentNullException(nameof(type), "Claim type cannot be null or whitespace.");

        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value), "Claim value cannot be null or whitespace.");

        var existingClaims = await roleManager.GetClaimsAsync(role);

        if (!existingClaims.Any(c => c.Type == type && c.Value == value))
        {
            await roleManager.AddClaimAsync(role, new Claim(type, value));
        }
    }

    /// <summary>
    /// Adds a single permission claim to a role.
    /// </summary>
    /// <typeparam name="TRole">The type representing the application role.</typeparam>
    /// <param name="roleManager">The <see cref="RoleManager{TRole}"/> instance used to manage roles.</param>
    /// <param name="role">The role to which the permission claim should be added.</param>
    /// <param name="value">The permission value.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="roleManager"/>, <paramref name="role"/>, or <paramref name="value"/> is null or empty.</exception>
    /// <remarks>
    /// This method uses the <see cref="Security.Models.ClaimTypes.Permission"/> claim type.
    /// </remarks>
    public static async Task AddPermissionClaim<TRole>(
        this RoleManager<TRole> roleManager,
        TRole role,
        string value)
        where TRole : class
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value), "Permission value cannot be null or whitespace.");

        await roleManager.AddClaimAsync(role, Security.Models.ClaimTypes.Permission, value);
    }

    /// <summary>
    /// Adds multiple permission claims to a role.
    /// </summary>
    /// <typeparam name="TRole">The type representing the application role.</typeparam>
    /// <param name="roleManager">The <see cref="RoleManager{TRole}"/> instance used to manage roles.</param>
    /// <param name="role">The role to which permission claims should be added.</param>
    /// <param name="values">An array of permission values to add as claims.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="roleManager"/>, <paramref name="role"/>, or <paramref name="values"/> is null or empty.</exception>
    public static async Task AddPermissionClaims<TRole>(
        this RoleManager<TRole> roleManager,
        TRole role,
        params string[] values)
        where TRole : class
    {
        ArgumentNullException.ThrowIfNull(roleManager);
        ArgumentNullException.ThrowIfNull(role);

        if (values == null || values.Length == 0)
            throw new ArgumentException("At least one permission value must be provided.", nameof(values));

        foreach (var value in values)
        {
            await roleManager.AddPermissionClaim(role, value);
        }
    }
}