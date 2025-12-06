using NuvTools.Validation.Annotations;
using NuvTools.Resources;
using System.ComponentModel.DataAnnotations;

namespace NuvTools.Security.Identity.Models.Form;

/// <summary>
/// Represents the form data for changing a user's password.
/// </summary>
/// <remarks>
/// This form requires the current password, a new password, and confirmation of the new password.
/// Password complexity requirements include at least one capital letter, one lowercase letter, and one digit.
/// </remarks>
public class ChangePasswordForm
{
    /// <summary>
    /// Gets or sets the user's current password.
    /// </summary>
    /// <remarks>
    /// This field is required for verification before allowing a password change.
    /// Must be between 6 and 40 characters and meet complexity requirements.
    /// </remarks>
    [Display(Name = nameof(Fields.Password), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(Messages.XRequired), ErrorMessageResourceType = typeof(Messages))]
    [MaxLength(40, ErrorMessageResourceName = nameof(Messages.XMustHaveUpToYCharacters), ErrorMessageResourceType = typeof(Messages))]
    [MinLength(6, ErrorMessageResourceName = nameof(Messages.XMustHaveAtLeastYCharacters), ErrorMessageResourceType = typeof(Messages))]
    [PasswordComplexityCapitalLetters(1)]
    [PasswordComplexityLowerCaseLetters(1)]
    [PasswordComplexityDigits(1)]
    public virtual string? Password { get; set; }

    /// <summary>
    /// Gets or sets the new password the user wants to set.
    /// </summary>
    /// <remarks>
    /// This field is required and must be between 6 and 40 characters.
    /// Must contain at least one capital letter, one lowercase letter, and one digit.
    /// </remarks>
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
    /// This field is required and must match the <see cref="NewPassword"/> field exactly.
    /// </remarks>
    [Display(Name = nameof(Fields.ConfirmNewPassword), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(Messages.XRequired), ErrorMessageResourceType = typeof(Messages))]
    [Compare(nameof(NewPassword), ErrorMessageResourceName = nameof(Messages.XMustBeEqualY), ErrorMessageResourceType = typeof(Messages))]
    public virtual string? ConfirmNewPassword { get; set; }
}