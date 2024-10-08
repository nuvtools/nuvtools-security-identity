﻿using NuvTools.Resources;
using System.ComponentModel.DataAnnotations;

namespace NuvTools.Security.Identity.Models.Form;

public class UpdateProfileForm
{
    [Display(Name = nameof(Fields.Name), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(DynamicValidationMessages.XRequired), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    [MaxLength(50, ErrorMessageResourceName = nameof(DynamicValidationMessages.XMustHaveUpToYCharacters), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    public string? Name { get; set; }

    [Display(Name = nameof(Fields.Surname), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(DynamicValidationMessages.XRequired), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    [MaxLength(50, ErrorMessageResourceName = nameof(DynamicValidationMessages.XMustHaveUpToYCharacters), ErrorMessageResourceType = typeof(DynamicValidationMessages))]
    public string? Surname { get; set; }

    public string? Email { get; set; }

    public string? Token { get; set; }
}