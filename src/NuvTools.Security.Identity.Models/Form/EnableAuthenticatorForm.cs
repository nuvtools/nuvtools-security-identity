using NuvTools.Resources;
using System.ComponentModel.DataAnnotations;

namespace NuvTools.Security.Identity.Models.Form;

/// <summary>
/// Represents the confirmation that turns on two-factor authentication: the first code the user's
/// authenticator app produced after reading the shared key.
/// </summary>
/// <remarks>
/// Proving the app produces valid codes before enabling two-factor authentication is what keeps a
/// mistyped key from locking the user out.
/// </remarks>
public class EnableAuthenticatorForm
{
    /// <summary>
    /// Gets or sets the code shown by the authenticator app.
    /// </summary>
    /// <remarks>
    /// Six digits; up to seven characters so a code typed with a space still passes. Strip spaces and
    /// hyphens before verifying it.
    /// </remarks>
    [Display(Name = nameof(Fields.VerificationCode), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(Messages.XRequired), ErrorMessageResourceType = typeof(Messages))]
    [MinLength(6, ErrorMessageResourceName = nameof(Messages.XMustHaveAtLeastYCharacters), ErrorMessageResourceType = typeof(Messages))]
    [MaxLength(7, ErrorMessageResourceName = nameof(Messages.XMustHaveUpToYCharacters), ErrorMessageResourceType = typeof(Messages))]
    public virtual string? Code { get; set; }
}
