using NuvTools.Resources;
using System.ComponentModel.DataAnnotations;

namespace NuvTools.Security.Identity.Models.Form;

/// <summary>
/// Represents the second step of a sign-in when the user has no access to their authenticator app:
/// one of the recovery codes issued when two-factor authentication was enabled.
/// </summary>
/// <remarks>
/// Each recovery code works once. The user is identified by the sign-in in progress, never by
/// this form.
/// </remarks>
public class LoginWithRecoveryCodeForm
{
    /// <summary>
    /// Gets or sets the recovery code.
    /// </summary>
    /// <remarks>
    /// ASP.NET Core Identity issues codes of eleven characters with a hyphen in the middle; the limit
    /// leaves room for surrounding spaces, which should be trimmed before verifying it.
    /// </remarks>
    [Display(Name = nameof(Fields.RecoveryCode), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(Messages.XRequired), ErrorMessageResourceType = typeof(Messages))]
    [MaxLength(20, ErrorMessageResourceName = nameof(Messages.XMustHaveUpToYCharacters), ErrorMessageResourceType = typeof(Messages))]
    public virtual string? RecoveryCode { get; set; }
}
