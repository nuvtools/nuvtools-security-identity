using Microsoft.AspNetCore.Authorization;

namespace NuvTools.Security.Identity.Policy;
public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; private set; } = permission;
}