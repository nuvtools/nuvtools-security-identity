namespace NuvTools.Security.Identity.Models;

/// <summary>
/// Represents the association between a user and their assigned roles.
/// </summary>
/// <remarks>
/// This model is typically used for transferring or serializing role assignment data,  
/// for example in APIs or service layers that need to list or update user roles.  
/// It does not represent a database entity but rather a DTO (Data Transfer Object).
/// </remarks>
public class UserRoles
{
    /// <summary>
    /// Gets or sets the unique identifier of the user.
    /// </summary>
    /// <remarks>
    /// The identifier type is defined as <see cref="int"/> by default
    /// </remarks>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the list of role names assigned to the user.
    /// </summary>
    /// <remarks>
    /// Each string represents a role name.  
    /// If no roles are assigned, this property may be <see langword="null"/> or an empty list.
    /// </remarks>
    public IList<string>? Roles { get; set; }
}