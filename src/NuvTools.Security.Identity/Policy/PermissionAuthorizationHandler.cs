using Microsoft.AspNetCore.Authorization;
using NuvTools.Security.Models;

namespace NuvTools.Security.Identity.Policy;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    public PermissionAuthorizationHandler()
    { }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User is null)
            return Task.CompletedTask;

        var permissions = context.User.Claims.Where(x => x.Type == ClaimTypes.Permission &&
                                                            x.Value == requirement.Permission &&
                                                            x.Issuer == "LOCAL AUTHORITY");
        if (permissions.Any()) context.Succeed(requirement);

        return Task.CompletedTask;
    }
}