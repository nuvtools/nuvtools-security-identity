using Microsoft.AspNetCore.Authorization;

namespace NuvTools.Security.Identity.Policy;

/// <summary>
/// Represents an authorization requirement based on a specific permission.
/// </summary>
/// <remarks>
/// This requirement is evaluated by the <see cref="PermissionAuthorizationHandler"/> to determine
/// whether the current user possesses a claim that grants the specified permission.
/// </remarks>
public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    /// <summary>
    /// Gets the name or identifier of the permission required for authorization.
    /// </summary>
    /// <remarks>
    /// This value typically corresponds to the <c>Value</c> of a claim of type
    /// <c>ClaimTypes.Permission</c> that must be present on the authenticated user.
    /// </remarks>
    public string Permission { get; } =
        !string.IsNullOrWhiteSpace(permission)
            ? permission
            : throw new ArgumentNullException(nameof(permission), "Permission cannot be null or empty.");
}
