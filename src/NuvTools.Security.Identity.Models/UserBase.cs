using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using NuvTools.Validation.Annotations;
using NuvTools.Resources;

namespace NuvTools.Security.Identity.Models;

public abstract class UserBase<TKey> : IdentityUser<TKey> where TKey : IEquatable<TKey>
{
    [Display(Name = nameof(Fields.Email), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(DynamicValidationMessages.XRequired), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    [MaxLength(100, ErrorMessageResourceName = nameof(DynamicValidationMessages.XMustBeUpToYCharacters), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    [EmailAddress(ErrorMessageResourceName = nameof(Messages.InvalidEmail), ErrorMessageResourceType = typeof(Messages))]
    public override string Email { get => base.Email; set => base.Email = value; }

    [Display(Name = nameof(Fields.Name), ResourceType = typeof(Fields))]
    [MaxLength(50, ErrorMessageResourceName = nameof(DynamicValidationMessages.XMustBeUpToYCharacters), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    public virtual string Name { get; set; }

    [Display(Name = nameof(Fields.Surname), ResourceType = typeof(Fields))]
    [MaxLength(50, ErrorMessageResourceName = nameof(DynamicValidationMessages.XMustBeUpToYCharacters), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    public virtual string Surname { get; set; }

    [Display(Name = nameof(Fields.Status), ResourceType = typeof(Fields))]
    public virtual bool Status { get; set; }

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
    public string RefreshToken { get; set; }

    [NotMapped]
    public DateTime RefreshTokenExpiryTime { get; set; }
}