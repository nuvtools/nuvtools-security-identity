using System.ComponentModel.DataAnnotations;
using NuvTools.Resources;

namespace NuvTools.Security.Identity.Models.Form;

/// <summary>
/// Represents the form data for creating or updating a user profile.
/// </summary>
/// <remarks>
/// This form includes basic user information such as name, email, and account status.
/// It is commonly used for user management operations in administrative interfaces.
/// </remarks>
public class UserForm
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    /// <remarks>
    /// This is typically the primary key in the database. For new users, this may be 0 or uninitialized.
    /// </remarks>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    /// <remarks>
    /// This field is required and must be a valid email format with a maximum length of 100 characters.
    /// </remarks>
    [Display(Name = nameof(Fields.Email), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(Messages.XRequired), ErrorMessageResourceType = typeof(Messages))]
    [MaxLength(100, ErrorMessageResourceName = nameof(Messages.XMustHaveUpToYCharacters), ErrorMessageResourceType = typeof(Messages))]
    [EmailAddress(ErrorMessageResourceName = nameof(Messages.XInvalid), ErrorMessageResourceType = typeof(Messages))]
    public virtual string? Email { get; set; }

    /// <summary>
    /// Gets or sets the user's first name.
    /// </summary>
    /// <remarks>
    /// This field is required with a maximum length of 50 characters.
    /// </remarks>
    [Display(Name = nameof(Fields.Name), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(Messages.XRequired), ErrorMessageResourceType = typeof(Messages))]
    [MaxLength(50, ErrorMessageResourceName = nameof(Messages.XMustHaveUpToYCharacters), ErrorMessageResourceType = typeof(Messages))]
    public virtual string? Name { get; set; }

    /// <summary>
    /// Gets or sets the user's surname (last name).
    /// </summary>
    /// <remarks>
    /// This field is required with a maximum length of 50 characters.
    /// </remarks>
    [Display(Name = nameof(Fields.Surname), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(Messages.XRequired), ErrorMessageResourceType = typeof(Messages))]
    [MaxLength(50, ErrorMessageResourceName = nameof(Messages.XMustHaveUpToYCharacters), ErrorMessageResourceType = typeof(Messages))]
    public virtual string? Surname { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user account is active.
    /// </summary>
    /// <remarks>
    /// When <c>true</c>, the user account is active and can authenticate. When <c>false</c>, the account is disabled.
    /// </remarks>
    [Display(Name = nameof(Fields.Status), ResourceType = typeof(Fields))]
    public virtual bool Status { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user's email address has been confirmed.
    /// </summary>
    /// <remarks>
    /// This is typically set to <c>true</c> after the user verifies their email via a confirmation link.
    /// </remarks>
    public bool EmailConfirmed { get; set; }
}