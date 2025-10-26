using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using ClaimTypes = NuvTools.Security.Models.ClaimTypes;

namespace NuvTools.Security.Identity.Policy;

/// <summary>
/// Custom implementation of <see cref="IAuthorizationPolicyProvider"/> that dynamically generates
/// authorization policies based on permission claims.
/// </summary>
/// <remarks>
/// This provider enables runtime creation of policies following a naming convention.  
/// If a policy name starts with <see cref="ClaimTypes.Permission"/>, it creates a policy that requires
/// a <see cref="PermissionRequirement"/> with the corresponding value.  
/// Otherwise, it delegates to the default <see cref="DefaultAuthorizationPolicyProvider"/>.
/// </remarks>
public class AuthorizationPermissionPolicyProvider(IOptions<AuthorizationOptions> options) : IAuthorizationPolicyProvider
{
    /// <summary>
    /// The default fallback policy provider used for standard authorization policies.
    /// </summary>
    public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; } = new(options);

    /// <summary>
    /// Retrieves the default authorization policy.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> containing the default <see cref="AuthorizationPolicy"/>.
    /// </returns>
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
        FallbackPolicyProvider.GetDefaultPolicyAsync();

    /// <summary>
    /// Retrieves an authorization policy based on the specified policy name.
    /// </summary>
    /// <param name="policyName">The name of the policy to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> containing the <see cref="AuthorizationPolicy"/>, or <see langword="null"/> if none exists.
    /// </returns>
    /// <remarks>
    /// If the <paramref name="policyName"/> starts with <see cref="ClaimTypes.Permission"/>,  
    /// a new policy will be built dynamically that enforces a <see cref="PermissionRequirement"/>.  
    /// Otherwise, the request is delegated to the fallback provider.
    /// </remarks>
    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(policyName);

        // Handle dynamically generated permission-based policies
        if (policyName.StartsWith(ClaimTypes.Permission, StringComparison.OrdinalIgnoreCase))
        {
            var policyBuilder = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(policyName));

            return Task.FromResult<AuthorizationPolicy?>(policyBuilder.Build());
        }

        // Fallback to the default provider for other policies
        return FallbackPolicyProvider.GetPolicyAsync(policyName);
    }

    /// <summary>
    /// Retrieves the fallback authorization policy, if defined.
    /// </summary>
    /// <returns>
    /// Always returns <see langword="null"/> since this provider does not define a fallback policy.
    /// </returns>
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
        Task.FromResult<AuthorizationPolicy?>(null);
}
