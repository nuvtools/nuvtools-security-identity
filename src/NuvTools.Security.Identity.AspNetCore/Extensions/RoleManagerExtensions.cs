using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace NuvTools.Security.Identity.AspNetCore.Extensions;
public static class RoleManagerExtensions
{
    public static async Task AddClaimsAsync<TRole>(this RoleManager<TRole> roleManager, TRole role, IEnumerable<Claim> claims) where TRole : class
    {
        foreach (var item in claims)
        {
            await roleManager.AddClaimAsync(role, item);
        }
    }

    public static async Task AddClaimAsync<TRole>(this RoleManager<TRole> roleManager, TRole role, string type, string value) where TRole : class
    {
        var allClaims = await roleManager.GetClaimsAsync(role);
        if (!allClaims.Any(a => a.Type == type && a.Value == value))
        {
            await roleManager.AddClaimAsync(role, new Claim(type, value));
        }
    }

    public static async Task AddPermissionClaim<TRole>(this RoleManager<TRole> roleManager, TRole role, string value, string applicationId = null) where TRole : class
    {
        var claimType = string.IsNullOrEmpty(applicationId) ?
                        Security.Models.ClaimTypes.Permission :
                        string.Format(Security.Models.ClaimTypes.ApplicationPermission, applicationId);
        await roleManager.AddClaimAsync(role, claimType, value);
    }

    public static async Task AddPermissionClaims<TRole>(this RoleManager<TRole> roleManager, TRole role, params string[] values) where TRole : class
    {
        foreach (var item in values)
        {
            await roleManager.AddPermissionClaim(role, item);
        }
    }
}