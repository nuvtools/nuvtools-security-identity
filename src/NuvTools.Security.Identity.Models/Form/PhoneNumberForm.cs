using NuvTools.Resources;
using System.ComponentModel.DataAnnotations;

namespace NuvTools.Security.Identity.Models.Form;

/// <summary>
/// Represents a phone number the user wants on the account, before it is confirmed.
/// </summary>
/// <remarks>
/// Taken as typed — with or without the country calling code, with spaces, dashes or parentheses.
/// Normalize it (for instance with <c>E164PhoneNumber.TryNormalize</c> from NuvTools.Security) before
/// sending a code to it; the number is stored only once the code it received comes back.
/// </remarks>
public class PhoneNumberForm
{
    /// <summary>
    /// Gets or sets the phone number as the user typed it.
    /// </summary>
    [Display(Name = nameof(Fields.PhoneNumber), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(Messages.XRequired), ErrorMessageResourceType = typeof(Messages))]
    [MaxLength(25, ErrorMessageResourceName = nameof(Messages.XMustHaveUpToYCharacters), ErrorMessageResourceType = typeof(Messages))]
    public virtual string? PhoneNumber { get; set; }
}
