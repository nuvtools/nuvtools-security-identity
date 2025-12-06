using NuvTools.Resources;
using System.ComponentModel.DataAnnotations;

namespace NuvTools.Security.Identity.Models.Form;

/// <summary>
/// Represents the form data for updating a user's profile information.
/// </summary>
/// <remarks>
/// This form allows users to update their basic profile information such as name and surname.
/// It can also be used for email change operations when a token is provided.
/// </remarks>
public class UpdateProfileForm
{
    /// <summary>
    /// Gets or sets the user's first name.
    /// </summary>
    /// <remarks>
    /// This field is required with a maximum length of 50 characters.
    /// </remarks>
    [Display(Name = nameof(Fields.Name), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(Messages.XRequired), ErrorMessageResourceType = typeof(Messages))]
    [MaxLength(50, ErrorMessageResourceName = nameof(Messages.XMustHaveUpToYCharacters), ErrorMessageResourceType = typeof(Messages))]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the user's surname (last name).
    /// </summary>
    /// <remarks>
    /// This field is required with a maximum length of 50 characters.
    /// </remarks>
    [Display(Name = nameof(Fields.Surname), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(Messages.XRequired), ErrorMessageResourceType = typeof(Messages))]
    [MaxLength(50, ErrorMessageResourceName = nameof(Messages.XMustHaveUpToYCharacters), ErrorMessageResourceType = typeof(Messages))]
    public string? Surname { get; set; }

    /// <summary>
    /// Gets or sets the new email address when changing the user's email.
    /// </summary>
    /// <remarks>
    /// This field is optional and only used when the user is requesting an email change.
    /// A confirmation token must be provided via the <see cref="Token"/> property for email changes.
    /// </remarks>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the email change confirmation token.
    /// </summary>
    /// <remarks>
    /// This token is required when changing the user's email address.
    /// The token is typically sent to the new email address for verification.
    /// </remarks>
    public string? Token { get; set; }
}