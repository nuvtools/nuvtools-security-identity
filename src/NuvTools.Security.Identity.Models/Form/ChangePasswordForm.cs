using NuvTools.Validation.Annotations;
using NuvTools.Resources;
using System.ComponentModel.DataAnnotations;

namespace NuvTools.Security.Identity.Models.Form;

public class ChangePasswordForm
{
    [Display(Name = nameof(Fields.Password), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(DynamicValidationMessages.XRequired), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    [MaxLength(40, ErrorMessageResourceName = nameof(DynamicValidationMessages.XMustHaveUpToYCharacters), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    [MinLength(6, ErrorMessageResourceName = nameof(DynamicValidationMessages.XMustHaveAtLeastYCharacters), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    [PasswordComplexityCapitalLetters(1)]
    [PasswordComplexityLowerCaseLetters(1)]
    [PasswordComplexityDigits(1)]
    public virtual string? Password { get; set; }

    [Display(Name = nameof(Fields.NewPassword), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(DynamicValidationMessages.XRequired), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    [MaxLength(40, ErrorMessageResourceName = nameof(DynamicValidationMessages.XMustHaveUpToYCharacters), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    [MinLength(6, ErrorMessageResourceName = nameof(DynamicValidationMessages.XMustHaveAtLeastYCharacters), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    [PasswordComplexityCapitalLetters(1)]
    [PasswordComplexityLowerCaseLetters(1)]
    [PasswordComplexityDigits(1)]
    public virtual string? NewPassword { get; set; }

    [Display(Name = nameof(Fields.ConfirmNewPassword), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(DynamicValidationMessages.XRequired), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    [Compare(nameof(NewPassword), ErrorMessageResourceName = nameof(DynamicValidationMessages.XMustBeEqualY), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    public virtual string? ConfirmNewPassword { get; set; }
}