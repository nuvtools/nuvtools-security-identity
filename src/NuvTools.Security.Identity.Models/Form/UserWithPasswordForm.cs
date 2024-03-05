using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NuvTools.Validation.Annotations;
using NuvTools.Resources;

namespace NuvTools.Security.Identity.Models.Form;

public class UserWithPasswordForm : UserForm
{
    [NotMapped]
    [Display(Name = nameof(Fields.Password), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(DynamicValidationMessages.XRequired), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    [MaxLength(40, ErrorMessageResourceName = nameof(DynamicValidationMessages.XMustBeUpToYCharacters), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    [MinLength(6, ErrorMessageResourceName = nameof(DynamicValidationMessages.XShouldHaveAtLeastYCharacters), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    [PasswordComplexityCapitalLetters(1)]
    [PasswordComplexityLowerCaseLetters(1)]
    [PasswordComplexityDigits(1)]
    public virtual string Password { get; set; }

    [NotMapped]
    [Display(Name = nameof(Fields.PasswordConfirm), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(DynamicValidationMessages.XRequired), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    [Compare(nameof(Password), ErrorMessageResourceName = nameof(DynamicValidationMessages.XShouldBeEqualY), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    public virtual string ConfirmPassword { get; set; }
}