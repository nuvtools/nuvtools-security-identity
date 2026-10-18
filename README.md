# NuvTools.Security.Identity

[![NuGet](https://img.shields.io/nuget/v/NuvTools.Security.Identity.svg)](https://www.nuget.org/packages/NuvTools.Security.Identity)
[![License](https://img.shields.io/github/license/nuvtools/nuvtools-security-identity.svg)](LICENSE)

A modular library suite for ASP.NET Identity integration targeting .NET 8, .NET 9, and .NET 10. Provides identity models, permission-based authorization, user management services, and Entity Framework Core persistence.

## Libraries

| Library | Description | NuGet |
|---------|-------------|-------|
| **NuvTools.Security.Identity.Models** | Base models, form DTOs, and API models with localized validation | [![NuGet](https://img.shields.io/nuget/v/NuvTools.Security.Identity.Models.svg)](https://www.nuget.org/packages/NuvTools.Security.Identity.Models) |
| **NuvTools.Security.Identity** | Permission-based authorization with dynamic policy provider | [![NuGet](https://img.shields.io/nuget/v/NuvTools.Security.Identity.svg)](https://www.nuget.org/packages/NuvTools.Security.Identity) |
| **NuvTools.Security.Identity.AspNetCore** | User management service with email/password workflows and role administration | [![NuGet](https://img.shields.io/nuget/v/NuvTools.Security.Identity.AspNetCore.svg)](https://www.nuget.org/packages/NuvTools.Security.Identity.AspNetCore) |
| **NuvTools.Security.Identity.EntityFrameworkCore** | Identity DbContext with transaction management and batch operations | [![NuGet](https://img.shields.io/nuget/v/NuvTools.Security.Identity.EntityFrameworkCore.svg)](https://www.nuget.org/packages/NuvTools.Security.Identity.EntityFrameworkCore) |

## Installation

```bash
dotnet add package NuvTools.Security.Identity.Models
dotnet add package NuvTools.Security.Identity
dotnet add package NuvTools.Security.Identity.AspNetCore
dotnet add package NuvTools.Security.Identity.EntityFrameworkCore
```

## NuvTools.Security.Identity.Models

Base models, form DTOs, and API response models for ASP.NET Identity.

**Key Components:**
- `UserBase<TKey>` -- Abstract user entity extending `IdentityUser<TKey>` with Email, Name, Surname, Status, and password validation (6-40 chars, min 1 uppercase, 1 lowercase, 1 digit)
- `RoleBase<TKey>` -- Abstract role entity extending `IdentityRole<TKey>` with Name and Claims
- Form models: `LoginForm`, `UserForm`, `UserWithPasswordForm`, `ChangePasswordForm`, `ForgotPasswordForm`, `ResetPasswordForm`, `SetPasswordForm`, `UpdateProfileForm`
- Two-factor form models: `LoginWithTwoFactorForm`, `LoginWithRecoveryCodeForm`, `EnableAuthenticatorForm`
- API models: `TokenResponse`, `RefreshTokenRequest`
- `UserRoles` -- DTO for user-role associations

**Usage:**
```csharp
using NuvTools.Security.Identity.Models;

public class ApplicationUser : UserBase<Guid>
{
    // Add custom properties if needed
}

public class RegistrationDto : UserWithPasswordForm
{
    // Inherits Email, Name, Surname, Password, ConfirmPassword
    // with localized validation and password complexity requirements
}
```

## NuvTools.Security.Identity

Permission-based authorization with dynamic policy provider and claim-based permission handler.

**Key Components:**
- `AuthorizationPermissionPolicyProvider` -- Dynamically generates policies for names starting with "Permission."
- `PermissionRequirement` -- Authorization requirement carrying a permission string
- `PermissionAuthorizationHandler` -- Validates permission claims with issuer "LOCAL AUTHORITY"

**Usage:**
```csharp
using NuvTools.Security.Identity.Policy;

// Configure in Program.cs
builder.Services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPermissionPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

// Protect endpoints with permissions
[Authorize(Policy = "Permission.Users.Create")]
public IActionResult CreateUser() { }
```

## NuvTools.Security.Identity.AspNetCore

Server-side user management service and role administration. All methods return `IResult` / `IResult<T>` from the ResultWrapper pattern.

**Key Components:**
- `UserServiceBase<TUser, TRole, TKey>` -- Abstract service with user CRUD, email confirmation/change, password management, and role operations
- `RoleManagerExtensions` -- Extension methods for adding claims and permission claims to roles

**Available Operations:**
- User CRUD: `GetAllAsync`, `GetAsync`, `GetByEmailAsync`, `CreateAsync`, `UpdateAsync`, `DeleteAsync`
- User with roles: `CreateWithRolesAsync`, `GetRolesAsync`, `UpdateRolesAsync`
- Email: `GenerateEmailConfirmationTokenAsync`, `GenerateEmailConfirmationUriAsync`, `ConfirmEmailAsync`, `RequestChangeEmailUrlAsync`, `ChangeEmailAsync`
- Password: `ChangePasswordAsync`, `RequestResetPasswordUrlAsync`, `ResetPasswordAsync`
- Status: `ToggleUserStatusAsync`

**Usage:**
```csharp
using NuvTools.Security.Identity.AspNetCore.Services;
using NuvTools.Security.Identity.AspNetCore.Extensions;

public class UserService : UserServiceBase<ApplicationUser, ApplicationRole, Guid>
{
    public UserService(UserManager<ApplicationUser> userManager)
        : base(userManager) { }
}

// Add permission claims to roles
await roleManager.AddPermissionClaims(adminRole,
    "Permission.Users.Create",
    "Permission.Users.Read",
    "Permission.Users.Update",
    "Permission.Users.Delete"
);
```

## NuvTools.Security.Identity.EntityFrameworkCore

Entity Framework Core persistence layer with transaction management and batch operations.

**Key Components:**
- `IdentityDbContextBase<TUser, TRole, TIdentityKey>` -- Abstract DbContext extending `IdentityDbContext`, implementing `IDbContextCommands` and `IDbContextWithListCommands`
- Transaction management: `BeginTransactionAsync`, `CommitTransactionAsync`, `RollbackTransactionAsync`
- Execution strategy: `ExecuteWithStrategyAsync`
- CRUD: `AddAndSaveAsync`, `UpdateAndSaveAsync`, `RemoveAndSaveAsync`
- Batch operations: `SyncFromListAsync`, `AddOrUpdateFromListAsync`, `AddOrRemoveFromListAsync`

**Usage:**
```csharp
using NuvTools.Security.Identity.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContextBase<ApplicationUser, ApplicationRole, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Configure your entities
    }
}
```

## Building

This solution uses the `.slnx` (XML-based solution) format.

```bash
dotnet build NuvTools.Security.Identity.slnx
dotnet build NuvTools.Security.Identity.slnx -c Release
```

## Releasing

Publishing to nuget.org is irreversible per version, so the workflow makes it the last and smallest
step.

**One workflow per library.** Each package has its own, so a run and an approval each concern
exactly one package. The steps themselves are shared, in the reusable `package-publish.yml`.

| Package | Workflow | Release tag |
|---|---|---|
| `NuvTools.Security.Identity.Models` | `models-publish.yml` | `models-vX.Y.Z` |
| `NuvTools.Security.Identity` | `identity-publish.yml` | `identity-vX.Y.Z` |
| `NuvTools.Security.Identity.AspNetCore` | `aspnetcore-publish.yml` | `aspnetcore-vX.Y.Z` |
| `NuvTools.Security.Identity.EntityFrameworkCore` | `entityframeworkcore-publish.yml` | `entityframeworkcore-vX.Y.Z` |

1. Set the package's `<Version>` in its `.csproj` and merge it. That is the version that gets
   published; the workflow reads it and never computes one.
2. Run the library's workflow by hand (*Actions → Models Publish → Run workflow*) with *push*
   checked.
3. The workflow builds the solution with `-warnaserror`, runs the tests, takes **that package only**
   from the build output, verifies it carries its README, licence, icon, symbols and every target
   framework, and then waits for approval on the `nuget` GitHub Environment before pushing.
4. **After the push succeeds, the workflow tags the commit** with the tag in the table above. The tag is a
   record of what is on nuget.org, never what starts a release, so it cannot exist for a version
   that did not get there. A run whose tag already exists stops before building: the version has
   been released and `<Version>` needs a bump.

**`NuvTools.Security.Identity.Models` goes before `NuvTools.Security.Identity.AspNetCore`.** The
latter references it as a project, so it depends on whatever `<Version>` Models has in that commit.
The workflow checks that version is on nuget.org and refuses to push a package whose dependency is
not there yet.

That check knows a version exists, not what is in it. **When Models gains a type AspNetCore uses,
bump its `<Version>` and publish it first** — otherwise AspNetCore is published against an older
Models that does not have it, and fails for whoever installs it.

**Versions published by hand are refused.** Releases made before these workflows exist on nuget.org
with no tag. Pushing one again would skip the duplicate and then tag the current commit as a
release it is not, so a version that is already on nuget.org stops the run. The exception is a
re-run of the same run, which is how a push that succeeded and a tag that failed gets finished.

**No API key is stored.** The push uses nuget.org's trusted publishing: the job presents a GitHub
OIDC token, nuget.org checks it against the policy registered for this repository and answers with a
key that lives for one hour. The repository needs one secret, `NUGET_USER` — the nuget.org profile
name the policy belongs to — and the policy itself, registered under *Trusted Publishing* on
nuget.org with the owner `nuvtools`, the repository `nuvtools-security-identity`, the workflow file
and the environment `nuget`.

Run a library's workflow by hand with *push* unchecked for a dry run that stops after the
verification.

## Project Structure

```
nuvtools-security-identity/
├── src/
│   ├── NuvTools.Security.Identity.Models/
│   ├── NuvTools.Security.Identity/
│   ├── NuvTools.Security.Identity.AspNetCore/
│   └── NuvTools.Security.Identity.EntityFrameworkCore/
├── NuvTools.Security.Identity.slnx
└── README.md
```

## License

This project is licensed under the terms specified in the [LICENSE](LICENSE) file.

## Links

- [Website](https://nuvtools.com)
- [GitHub Repository](https://github.com/nuvtools/nuvtools-security-identity)
- [NuGet Packages](https://www.nuget.org/profiles/NuvTools)
