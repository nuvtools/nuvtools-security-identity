using NuvTools.Validation.Annotations;
using NuvTools.Resources;
using System.ComponentModel.DataAnnotations;

namespace NuvTools.Security.Identity.Models.Form;

/// <summary>
/// Represents a new password chosen without typing the current one: completing a reset link, or
/// adding a password to an account that signed up through an external provider.
/// </summary>
/// <remarks>
/// The user and the proof that they may set it — the reset token, or the signed-in session — come
/// from the link or the session, never from this form. Password complexity requirements include at
/// least one capital letter, one lowercase letter, and one digit; the identity system's own policy
/// still runs when the password is applied.
/// </remarks>
public class SetPasswordForm
{
    /// <summary>
    /// Gets or sets the new password.
    /// </summary>
    [Display(Name = nameof(Fields.NewPassword), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(Messages.XRequired), ErrorMessageResourceType = typeof(Messages))]
    [MaxLength(40, ErrorMessageResourceName = nameof(Messages.XMustHaveUpToYCharacters), ErrorMessageResourceType = typeof(Messages))]
    [MinLength(6, ErrorMessageResourceName = nameof(Messages.XMustHaveAtLeastYCharacters), ErrorMessageResourceType = typeof(Messages))]
    [PasswordComplexityCapitalLetters(1)]
    [PasswordComplexityLowerCaseLetters(1)]
    [PasswordComplexityDigits(1)]
    public virtual string? NewPassword { get; set; }

    /// <summary>
    /// Gets or sets the confirmation of the new password.
    /// </summary>
    /// <remarks>
    /// Must match <see cref="NewPassword"/> exactly.
    /// </remarks>
    [Display(Name = nameof(Fields.ConfirmNewPassword), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(Messages.XRequired), ErrorMessageResourceType = typeof(Messages))]
    [Compare(nameof(NewPassword), ErrorMessageResourceName = nameof(Messages.XMustBeEqualY), ErrorMessageResourceType = typeof(Messages))]
    public virtual string? ConfirmNewPassword { get; set; }
}
