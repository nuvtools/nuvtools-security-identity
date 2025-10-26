using Microsoft.AspNetCore.Authorization;
using ClaimTypes = NuvTools.Security.Models.ClaimTypes;

namespace NuvTools.Security.Identity.Policy;

/// <summary>
/// Handles authorization based on permission claims.
/// </summary>
/// <remarks>
/// This handler evaluates whether the current authenticated user possesses a required permission
/// (represented by a claim of type <see cref="ClaimTypes.Permission"/>).  
/// If the user has the required permission, the authorization requirement is marked as succeeded.
/// </remarks>
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionAuthorizationHandler"/> class.
    /// </summary>
    public PermissionAuthorizationHandler()
    {
    }

    /// <summary>
    /// Handles authorization logic for the specified <see cref="PermissionRequirement"/>.
    /// </summary>
    /// <param name="context">The authorization context containing user information and requirements.</param>
    /// <param name="requirement">The permission requirement being evaluated.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// The handler checks for a claim of type <see cref="ClaimTypes.Permission"/> that matches
    /// the required permission and has the issuer <c>"LOCAL AUTHORITY"</c>.
    /// </remarks>
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);

        // No user or identity available
        if (context.User?.Identity is not { IsAuthenticated: true })
            return Task.CompletedTask;

        // Find matching permission claims
        var hasPermission = context.User.Claims.Any(c =>
            c.Type == ClaimTypes.Permission &&
            string.Equals(c.Value, requirement.Permission, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(c.Issuer, "LOCAL AUTHORITY", StringComparison.OrdinalIgnoreCase));

        if (hasPermission)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
