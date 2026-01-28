using NuvTools.Resources;
using System.ComponentModel.DataAnnotations;

namespace NuvTools.Security.Identity.Models.Form;

/// <summary>
/// Represents the form data for user login authentication.
/// </summary>
/// <remarks>
/// This form is typically used for authenticating users via email and password credentials.
/// It includes validation attributes for input verification.
/// </remarks>
public class LoginForm
{
    /// <summary>
    /// Gets or sets the user's email address for login.
    /// </summary>
    /// <remarks>
    /// This field is required and must be a valid email format with a maximum length of 50 characters.
    /// </remarks>
    [Display(Name = nameof(Fields.Email), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(Messages.XRequired), ErrorMessageResourceType = typeof(Messages))]
    [MaxLength(50, ErrorMessageResourceName = nameof(Messages.XMustHaveUpToYCharacters), ErrorMessageResourceType = typeof(Messages))]
    [EmailAddress(ErrorMessageResourceName = nameof(Messages.XInvalid), ErrorMessageResourceType = typeof(Messages))]
    public virtual string? Email { get; set; }

    /// <summary>
    /// Gets or sets the user's password for login.
    /// </summary>
    /// <remarks>
    /// This field is required with a maximum length of 50 characters.
    /// </remarks>
    [Display(Name = nameof(Fields.Password), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(Messages.XRequired), ErrorMessageResourceType = typeof(Messages))]
    [MaxLength(50, ErrorMessageResourceName = nameof(Messages.XMustHaveUpToYCharacters), ErrorMessageResourceType = typeof(Messages))]
    public virtual string? Password { get; set; }
}