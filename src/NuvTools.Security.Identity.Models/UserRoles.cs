namespace NuvTools.Security.Identity.Models;

public class UserRoles
{
    public int UserId { get; set; }
    public IList<string>? Roles { get; set; }
}