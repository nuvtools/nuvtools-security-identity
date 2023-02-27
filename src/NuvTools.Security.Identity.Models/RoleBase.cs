using Microsoft.AspNetCore.Identity;
using NuvTools.Resources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NuvTools.Security.Identity.Models;

public abstract class RoleBase<TKey> : IdentityRole<TKey> where TKey : IEquatable<TKey>
{
    public RoleBase() : base()
    {
    }

    public RoleBase(string name) : base(name)
    {
    }

    [Display(Name = nameof(Fields.Name), ResourceType = typeof(Fields))]
    [Required(ErrorMessageResourceName = nameof(Messages.XRequired), ErrorMessageResourceType = typeof(Messages))]
    [MaxLength(30, ErrorMessageResourceName = nameof(Messages.XMustBeUpToYCharacters), ErrorMessageResourceType = typeof(Messages))]
    public override string Name { get => base.Name; set => base.Name = value; }

    [NotMapped]
    public List<KeyValuePair<string, string>> Claims { get; set; }
}