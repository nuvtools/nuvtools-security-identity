using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using NuvTools.Validation.Annotations;
using NuvTools.Resources;

namespace NuvTools.Security.Identity.Models;

/// <summary>
/// Represents the base class for application users within the identity system.
/// </summary>
/// <typeparam name="TKey">The type of the user's primary key (e.g., <see cref="Guid"/> or <see cref="int"/>).</typeparam>
/// <remarks>
/// This abstract class extends <see cref="IdentityUser{TKey}"/> to include
/// additional profile data and validation attributes for multilingual applications.
/// It also introduces password and refresh token fields that are not persisted in the database.
/// </remarks>
public abstract class UserBase<TKey> : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    /// <remarks>
    /// This property overrides the base <see cref="IdentityUser{TKey}.Email"/> property
    /// to add localized display names and validation attributes.
    /// </remarks>
    [Display(Name = nameof(Fields.Email), ResourceType = typeof(Fields))]
    [Required(
        ErrorMessageResourceName = nameof(Messages.XRequired),
        ErrorMessageResourceType = typeof(Messages))]
    [MaxLength(
        100,
        ErrorMessageResourceName = nameof(Messages.XMustHaveUpToYCharacters),
        ErrorMessageResourceType = typeof(Messages))]
    [EmailAddress(
        ErrorMessageResourceName = nameof(Messages.InvalidEmail),
        ErrorMessageResourceType = typeof(Messages))]
    public override string? Email
    {
        get => base.Email;
        set => base.Email = value;
    }

    /// <summary>
    /// Gets or sets the user's first name.
    /// </summary>
    [Display(Name = nameof(Fields.Name), ResourceType = typeof(Fields))]
    [MaxLength(
        50,
        ErrorMessageResourceName = nameof(Messages.XMustHaveUpToYCharacters),
        ErrorMessageResourceType = typeof(Messages))]
    public virtual string? Name { get; set; }

    /// <summary>
    /// Gets or sets the user's last name or surname.
    /// </summary>
    [Display(Name = nameof(Fields.Surname), ResourceType = typeof(Fields))]
    [MaxLength(
        50,
        ErrorMessageResourceName = nameof(Messages.XMustHaveUpToYCharacters),
        ErrorMessageResourceType = typeof(Messages))]
    public virtual string? Surname { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user account is active.
    /// </summary>
    [Display(Name = nameof(Fields.Status), ResourceType = typeof(Fields))]
    public virtual bool Status { get; set; }

    /// <summary>
    /// Gets or sets the user's password.  
    /// This property is **not persisted** in the database.
    /// </summary>
    /// <remarks>
    /// It is intended for use during user registration or password updates.
    /// The complexity attributes are enforced through <see cref="NuvTools.Validation.Annotations"/>.
    /// </remarks>
    [NotMapped]
    [Display(Name = nameof(Fields.Password), ResourceType = typeof(Fields))]
    [Required(
        ErrorMessageResourceName = nameof(Messages.XRequired),
        ErrorMessageResourceType = typeof(Messages))]
    [MaxLength(
        40,
        ErrorMessageResourceName = nameof(Messages.XMustHaveUpToYCharacters),
        ErrorMessageResourceType = typeof(Messages))]
    [MinLength(
        6,
        ErrorMessageResourceName = nameof(Messages.XMustHaveAtLeastYCharacters),
        ErrorMessageResourceType = typeof(Messages))]
    [PasswordComplexityCapitalLetters(1)]
    [PasswordComplexityLowerCaseLetters(1)]
    [PasswordComplexityDigits(1)]
    public virtual string? Password { get; set; }

    /// <summary>
    /// Gets or sets the refresh token used for renewing authentication sessions.
    /// </summary>
    /// <remarks>
    /// This property is not mapped to the database and is meant for temporary runtime storage.
    /// </remarks>
    [NotMapped]
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the refresh token expires.
    /// </summary>
    /// <remarks>
    /// This property is not persisted in the database.
    /// </remarks>
    [NotMapped]
    public DateTime RefreshTokenExpiryTime { get; set; }
}