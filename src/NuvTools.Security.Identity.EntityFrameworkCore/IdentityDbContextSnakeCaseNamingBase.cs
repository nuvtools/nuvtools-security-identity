using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace NuvTools.Security.Identity.EntityFrameworkCore;

public class IdentityIntDbContextSnakeCaseNaming : IdentityDbContextSnakeCaseNamingBase<IdentityUser<int>, IdentityRole<int>, int>;

public class IdentityGuidDbContextSnakeCaseNaming : IdentityDbContextSnakeCaseNamingBase<IdentityUser<Guid>, IdentityRole<Guid>, Guid>;

public class IdentityIntDbContextSnakeCaseNaming<TUser, TRole> : IdentityDbContextSnakeCaseNamingBase<TUser, TRole, int>
                                                                                where TUser : IdentityUser<int>
                                                                                where TRole : IdentityRole<int>;

public class IdentityGuidDbContextSnakeCaseNaming<TUser, TRole> : IdentityDbContextSnakeCaseNamingBase<TUser, TRole, Guid>
                                                                        where TUser : IdentityUser<Guid>
                                                                        where TRole : IdentityRole<Guid>;

public abstract class IdentityDbContextSnakeCaseNamingBase<TUser, TRole, TIdentityKey> : IdentityDbContextBase<TUser, TRole, TIdentityKey>
                                                                                            where TUser : IdentityUser<TIdentityKey>
                                                                                            where TRole : IdentityRole<TIdentityKey>
                                                                                            where TIdentityKey : IEquatable<TIdentityKey>
{
    protected IdentityDbContextSnakeCaseNamingBase()
    {
    }

    protected IdentityDbContextSnakeCaseNamingBase(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        foreach (var entity in builder.Model.GetEntityTypes())
        {
            if (entity.GetTableName() is { } tableName)
                entity.SetTableName(ToSnakeCase(tableName));

            if (entity.GetSchema() is { } schemaName)
                entity.SetSchema(ToSnakeCase(schemaName));
        }
    }

    private static string ToSnakeCase(string name)
    {
        return string.Concat(
            name.Select((c, i) => i > 0 && char.IsUpper(c) ? "_" + char.ToLower(c) : char.ToLower(c).ToString())
        );
    }
}
