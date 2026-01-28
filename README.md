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
- Form models: `LoginForm`, `UserForm`, `UserWithPasswordForm`, `ChangePasswordForm`, `ForgotPasswordForm`, `ResetPasswordForm`, `UpdateProfileForm`
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
