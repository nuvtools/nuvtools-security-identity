﻿using NuvTools.Resources;
using System.ComponentModel.DataAnnotations;

namespace NuvTools.Security.Identity.Models.Form;
public class LoginForm
{
    [Display(Name = nameof(Fields.Email), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(DynamicValidationMessages.XRequired), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    [MaxLength(50, ErrorMessageResourceName = nameof(DynamicValidationMessages.XMustBeUpToYCharacters), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    [EmailAddress(ErrorMessageResourceName = nameof(Messages.InvalidEmail), ErrorMessageResourceType = typeof(Messages))]
    public virtual string Email { get; set; }

    [Display(Name = nameof(Fields.Password), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(DynamicValidationMessages.XRequired), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    [MaxLength(50, ErrorMessageResourceName = nameof(DynamicValidationMessages.XMustBeUpToYCharacters), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    public virtual string Password { get; set; }
}