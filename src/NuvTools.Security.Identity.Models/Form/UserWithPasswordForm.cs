using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NuvTools.Validation.Annotations;
using NuvTools.Resources;

namespace NuvTools.Security.Identity.Models.Form;

/// <summary>
/// Represents the form data for creating or updating a user with password information.
/// </summary>
/// <remarks>
/// This form extends <see cref="UserForm"/> to include password fields for user registration scenarios.
/// Password complexity requirements include at least one capital letter, one lowercase letter, and one digit.
/// The password fields are not mapped to the database and are only used during the registration process.
/// </remarks>
public class UserWithPasswordForm : UserForm
{
    /// <summary>
    /// Gets or sets the user's password.
    /// </summary>
    /// <remarks>
    /// This field is required and must be between 6 and 40 characters.
    /// Must contain at least one capital letter, one lowercase letter, and one digit.
    /// This property is not persisted to the database.
    /// </remarks>
    [NotMapped]
    [Display(Name = nameof(Fields.Password), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(Messages.XRequired), ErrorMessageResourceType = typeof(Messages))]
    [MaxLength(40, ErrorMessageResourceName = nameof(Messages.XMustHaveUpToYCharacters), ErrorMessageResourceType = typeof(Messages))]
    [MinLength(6, ErrorMessageResourceName = nameof(Messages.XMustHaveAtLeastYCharacters), ErrorMessageResourceType = typeof(Messages))]
    [PasswordComplexityCapitalLetters(1)]
    [PasswordComplexityLowerCaseLetters(1)]
    [PasswordComplexityDigits(1)]
    public virtual string? Password { get; set; }

    /// <summary>
    /// Gets or sets the password confirmation field.
    /// </summary>
    /// <remarks>
    /// This field is required and must match the <see cref="Password"/> field exactly.
    /// This property is not persisted to the database.
    /// </remarks>
    [NotMapped]
    [Display(Name = nameof(Fields.PasswordConfirm), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(Messages.XRequired), ErrorMessageResourceType = typeof(Messages))]
    [Compare(nameof(Password), ErrorMessageResourceName = nameof(Messages.XMustBeEqualY), ErrorMessageResourceType = typeof(Messages))]
    public virtual string? ConfirmPassword { get; set; }
}