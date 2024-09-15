using System.ComponentModel.DataAnnotations;
using NuvTools.Resources;

namespace NuvTools.Security.Identity.Models.Form;
public class UserForm
{
    public int Id { get; set; }

    [Display(Name = nameof(Fields.Email), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(DynamicValidationMessages.XRequired), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    [MaxLength(100, ErrorMessageResourceName = nameof(DynamicValidationMessages.XMustHaveUpToYCharacters), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    [EmailAddress(ErrorMessageResourceName = nameof(Messages.InvalidEmail), ErrorMessageResourceType = typeof(Messages))]
    public virtual string? Email { get; set; }

    [Display(Name = nameof(Fields.Name), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(DynamicValidationMessages.XRequired), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    [MaxLength(50, ErrorMessageResourceName = nameof(DynamicValidationMessages.XMustHaveUpToYCharacters), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    public virtual string? Name { get; set; }

    [Display(Name = nameof(Fields.Surname), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(DynamicValidationMessages.XRequired), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    [MaxLength(50, ErrorMessageResourceName = nameof(DynamicValidationMessages.XMustHaveUpToYCharacters), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    public virtual string? Surname { get; set; }

    [Display(Name = nameof(Fields.Status), ResourceType = typeof(Fields))]
    public virtual bool Status { get; set; }

    public bool EmailConfirmed { get; set; }
}