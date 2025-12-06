using NuvTools.Resources;
using System.ComponentModel.DataAnnotations;

namespace NuvTools.Security.Identity.Models.Form;

/// <summary>
/// Represents the form data for initiating a password reset request.
/// </summary>
/// <remarks>
/// This form is used when a user forgets their password and needs to request a password reset link.
/// A password reset token will be generated and sent to the provided email address.
/// </remarks>
public class ForgotPasswordForm
{
    /// <summary>
    /// Gets or sets the email address of the user requesting a password reset.
    /// </summary>
    /// <remarks>
    /// This field is required and must be a valid email format with a maximum length of 50 characters.
    /// The email must be associated with a confirmed user account to receive the reset link.
    /// </remarks>
    [Display(Name = nameof(Fields.Email), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(Messages.XRequired), ErrorMessageResourceType = typeof(Messages))]
    [MaxLength(50, ErrorMessageResourceName = nameof(Messages.XMustHaveUpToYCharacters), ErrorMessageResourceType = typeof(Messages))]
    [EmailAddress(ErrorMessageResourceName = nameof(Messages.InvalidEmail), ErrorMessageResourceType = typeof(Messages))]
    public string? Email { get; set; }
}