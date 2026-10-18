using NuvTools.Resources;
using System.ComponentModel.DataAnnotations;

namespace NuvTools.Security.Identity.Models.Form;

/// <summary>
/// Represents the second step of a sign-in: the code from the user's authenticator app.
/// </summary>
/// <remarks>
/// Used after the password has been accepted for a user with two-factor authentication enabled.
/// The user is identified by the sign-in in progress, never by this form.
/// </remarks>
public class LoginWithTwoFactorForm
{
    /// <summary>
    /// Gets or sets the code shown by the authenticator app.
    /// </summary>
    /// <remarks>
    /// Six digits. Up to seven characters are accepted so a code typed with a space in the middle,
    /// as most authenticator apps display it, still passes; strip spaces and hyphens before
    /// verifying it.
    /// </remarks>
    [Display(Name = nameof(Fields.VerificationCode), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(Messages.XRequired), ErrorMessageResourceType = typeof(Messages))]
    [MinLength(6, ErrorMessageResourceName = nameof(Messages.XMustHaveAtLeastYCharacters), ErrorMessageResourceType = typeof(Messages))]
    [MaxLength(7, ErrorMessageResourceName = nameof(Messages.XMustHaveUpToYCharacters), ErrorMessageResourceType = typeof(Messages))]
    public virtual string? Code { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this browser should skip the second step on later
    /// sign-ins.
    /// </summary>
    [Display(Name = nameof(Fields.RememberThisDeviceQuestion), ResourceType = typeof(Fields))]
    public virtual bool RememberDevice { get; set; }
}
