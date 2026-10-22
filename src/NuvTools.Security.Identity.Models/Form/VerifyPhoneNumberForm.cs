using NuvTools.Resources;
using System.ComponentModel.DataAnnotations;

namespace NuvTools.Security.Identity.Models.Form;

/// <summary>
/// Represents the code a phone number received, typed back to confirm the number.
/// </summary>
/// <remarks>
/// The number itself is never taken from this form: it is the one the code was sent to, kept by the
/// server between the two steps.
/// </remarks>
public class VerifyPhoneNumberForm
{
    /// <summary>
    /// Gets or sets the code the text message carried.
    /// </summary>
    /// <remarks>
    /// Four to ten digits, as providers send them; up to eleven characters so a code typed with a
    /// space still passes. Strip spaces and hyphens before verifying it.
    /// </remarks>
    [Display(Name = nameof(Fields.VerificationCode), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(Messages.XRequired), ErrorMessageResourceType = typeof(Messages))]
    [MinLength(4, ErrorMessageResourceName = nameof(Messages.XMustHaveAtLeastYCharacters), ErrorMessageResourceType = typeof(Messages))]
    [MaxLength(11, ErrorMessageResourceName = nameof(Messages.XMustHaveUpToYCharacters), ErrorMessageResourceType = typeof(Messages))]
    public virtual string? Code { get; set; }
}
