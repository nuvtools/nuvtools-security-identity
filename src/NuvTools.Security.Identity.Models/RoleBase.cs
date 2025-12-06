using Microsoft.AspNetCore.Identity;
using NuvTools.Resources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NuvTools.Security.Identity.Models;

/// <summary>
/// Represents the base model for application roles within the identity system.
/// </summary>
/// <typeparam name="TKey">The type of the primary key used for the role (e.g., <see cref="Guid"/> or <see cref="int"/>).</typeparam>
/// <remarks>
/// This abstract class extends <see cref="IdentityRole{TKey}"/> to provide additional
/// validation, display metadata, and support for storing associated claims in memory.
/// </remarks>
public abstract class RoleBase<TKey> : IdentityRole<TKey> where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RoleBase{TKey}"/> class.
    /// </summary>
    protected RoleBase() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RoleBase{TKey}"/> class with the specified name.
    /// </summary>
    /// <param name="name">The name of the role.</param>
    protected RoleBase(string name) : base(name)
    {
    }

    /// <summary>
    /// Gets or sets the display name of the role.
    /// </summary>
    /// <remarks>
    /// This property overrides the base <see cref="IdentityRole{TKey}.Name"/> property
    /// to apply validation and localization-friendly display attributes.
    /// </remarks>
    [Display(
        Name = nameof(Fields.Name),
        ResourceType = typeof(Fields))]
    [Required(
        ErrorMessageResourceName = nameof(Messages.XRequired),
        ErrorMessageResourceType = typeof(Messages))]
    [MaxLength(
        30,
        ErrorMessageResourceName = nameof(Messages.XMustHaveUpToYCharacters),
        ErrorMessageResourceType = typeof(Messages))]
    public override string? Name
    {
        get => base.Name;
        set => base.Name = value;
    }

    /// <summary>
    /// Gets or sets the list of claims associated with this role.
    /// </summary>
    /// <remarks>
    /// This property is marked as "NotMapped" since claims are managed
    /// through the <see cref="RoleManager{TRole}"/> API and are not persisted directly
    /// in the database.
    /// </remarks>
    [NotMapped]
    public List<KeyValuePair<string, string>>? Claims { get; set; }
}