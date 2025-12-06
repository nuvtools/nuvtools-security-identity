# Nuv Tools Security - ASP.NET Identity Libraries

[![NuGet](https://img.shields.io/nuget/v/NuvTools.Security.Identity.svg)](https://www.nuget.org/packages/NuvTools.Security.Identity)
[![License](https://img.shields.io/github/license/nuvtools/nuvtools-security-identity.svg)](LICENSE)

Nuv Tools Security provides a modular set of libraries designed to simplify and enhance ASP.NET Identity integration for modern .NET applications. These libraries target **.NET 8**, **.NET 9**, and **.NET 10**, offering solutions for identity models, helper utilities, ASP.NET Core integration, and Entity Framework Core support.

---

## 📦 Libraries

Nuv Tools Security is organized into four main libraries:

| Library | Purpose | NuGet |
|---------|---------|-------|
| **NuvTools.Security.Identity.Models** | Identity models and forms | [![NuGet](https://img.shields.io/nuget/v/NuvTools.Security.Identity.Models.svg)](https://www.nuget.org/packages/NuvTools.Security.Identity.Models) |
| **NuvTools.Security.Identity** | Authorization helpers and utilities | [![NuGet](https://img.shields.io/nuget/v/NuvTools.Security.Identity.svg)](https://www.nuget.org/packages/NuvTools.Security.Identity) |
| **NuvTools.Security.Identity.AspNetCore** | ASP.NET Core integration | [![NuGet](https://img.shields.io/nuget/v/NuvTools.Security.Identity.AspNetCore.svg)](https://www.nuget.org/packages/NuvTools.Security.Identity.AspNetCore) |
| **NuvTools.Security.Identity.EntityFrameworkCore** | EF Core persistence layer | [![NuGet](https://img.shields.io/nuget/v/NuvTools.Security.Identity.EntityFrameworkCore.svg)](https://www.nuget.org/packages/NuvTools.Security.Identity.EntityFrameworkCore) |

Each library is designed for modular use, allowing you to include only the components relevant to your project.

---

## 🚀 Features

- **Multi-Framework Support**: Targets .NET 8, .NET 9, and .NET 10
- **Localized Validation**: Built-in resource-based validation messages for multilingual applications
- **Permission-Based Authorization**: Dynamic policy generation for fine-grained access control
- **Password Complexity**: Configurable password requirements with custom validation attributes
- **User Management Service**: Comprehensive user operations including CRUD, email confirmation, and password management
- **Token Management**: Built-in support for JWT and refresh token workflows
- **Result Pattern**: Standardized `IResult` and `IResult<T>` return types instead of exceptions
- **EF Core Extensions**: Batch operations and transaction support for identity persistence
- **Form Models**: Ready-to-use DTOs for common identity operations (login, registration, password reset, etc.)

---

## 📥 Installation

Install the packages via NuGet Package Manager:

```bash
# Install the models library
dotnet add package NuvTools.Security.Identity.Models

# Install the authorization helpers
dotnet add package NuvTools.Security.Identity

# Install ASP.NET Core integration
dotnet add package NuvTools.Security.Identity.AspNetCore

# Install EF Core support
dotnet add package NuvTools.Security.Identity.EntityFrameworkCore
```

---

## 📖 Library Details

### 1. NuvTools.Security.Identity.Models

Provides base models, form DTOs, and API response models for ASP.NET Identity.

**Target Frameworks:** .NET 8, .NET 9, .NET 10

**Key Components:**
- `UserBase<TKey>` - Base class for user entities with profile data and validation
- `RoleBase<TKey>` - Base class for role entities with claims support
- Form Models: `LoginForm`, `UserForm`, `ChangePasswordForm`, `ForgotPasswordForm`, `ResetPasswordForm`, `UpdateProfileForm`, `UserWithPasswordForm`
- API Models: `TokenResponse`, `RefreshTokenRequest`
- `UserRoles` - DTO for user-role associations

**Usage Example:**
```csharp
using NuvTools.Security.Identity.Models;

// Define your user entity
public class ApplicationUser : UserBase<Guid>
{
    // Add custom properties if needed
}

// Use in registration
public class RegistrationDto : UserWithPasswordForm
{
    // Inherits Email, Name, Surname, Password, ConfirmPassword
    // with validation and password complexity requirements
}
```

### 2. NuvTools.Security.Identity

Helper library offering permission-based authorization and policy providers.

**Target Frameworks:** .NET 8, .NET 9, .NET 10

**Key Components:**
- `AuthorizationPermissionPolicyProvider` - Dynamic policy generation based on permission claims
- `PermissionRequirement` - Authorization requirement for permission-based access
- `PermissionAuthorizationHandler` - Handles permission claim validation

**Usage Example:**
```csharp
using NuvTools.Security.Identity.Policy;
using Microsoft.AspNetCore.Authorization;

// Configure in Program.cs
builder.Services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPermissionPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

// Use in controllers
[Authorize(Policy = "Permission.Users.Create")]
public IActionResult CreateUser()
{
    // Only users with "Permission.Users.Create" claim can access
}
```

### 3. NuvTools.Security.Identity.AspNetCore

Server-side library providing user management services and role extensions.

**Target Frameworks:** .NET 8, .NET 9, .NET 10

**Key Components:**
- `UserServiceBase<TUser, TRole, TKey>` - Comprehensive user management operations
- `RoleManagerExtensions` - Extension methods for role and permission management

**Available Operations:**
- User CRUD (Create, Read, Update, Delete)
- Email confirmation and change
- Password reset and change
- Role assignment and management
- User status toggling

**Usage Example:**
```csharp
using NuvTools.Security.Identity.AspNetCore.Services;
using NuvTools.Security.Identity.AspNetCore.Extensions;

// Implement your user service
public class UserService : UserServiceBase<ApplicationUser, ApplicationRole, Guid>
{
    public UserService(UserManager<ApplicationUser> userManager)
        : base(userManager) { }
}

// Use in your application
public class AccountController : ControllerBase
{
    private readonly UserService _userService;

    public async Task<IActionResult> Register(UserWithPasswordForm model)
    {
        var user = new ApplicationUser
        {
            Email = model.Email,
            Name = model.Name,
            Surname = model.Surname,
            Password = model.Password
        };

        var result = await _userService.CreateAsync(user);

        if (result.Succeeded)
            return Ok(new { userId = result.Data });

        return BadRequest(result.Errors);
    }
}

// Add permission claims to roles
public async Task ConfigureRoles(RoleManager<ApplicationRole> roleManager)
{
    var adminRole = await roleManager.FindByNameAsync("Admin");

    await roleManager.AddPermissionClaims(adminRole,
        "Permission.Users.Create",
        "Permission.Users.Read",
        "Permission.Users.Update",
        "Permission.Users.Delete"
    );
}
```

### 4. NuvTools.Security.Identity.EntityFrameworkCore

Entity Framework Core extensions for identity persistence with advanced operations.

**Target Frameworks:** .NET 8, .NET 9, .NET 10

**Key Components:**
- `IdentityDbContextBase<TUser, TRole, TIdentityKey>` - Enhanced DbContext for identity
- Transaction support with execution strategies
- Batch operations for list synchronization

**Usage Example:**
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

// Use batch operations
public async Task UpdateUserRoles(Guid userId, List<string> newRoles)
{
    var userRoles = newRoles.Select(r => new IdentityUserRole<Guid>
    {
        UserId = userId,
        RoleId = GetRoleId(r)
    }).ToList();

    // Synchronize roles - adds new, removes missing
    await _context.SyncFromListAsync(
        userRoles,
        ur => ur.RoleId,
        ur => ur.UserId == userId
    );

    await _context.SaveChangesAsync();
}
```

---

## 🔑 Key Features Explained

### Permission-Based Authorization

The permission system allows dynamic policy creation based on claims:

```csharp
// Assign permissions to roles
await roleManager.AddPermissionClaim(adminRole, "Permission.Users.Manage");
await roleManager.AddPermissionClaim(editorRole, "Permission.Content.Edit");

// Protect endpoints with permissions
[Authorize(Policy = "Permission.Users.Manage")]
public IActionResult ManageUsers() { }

[Authorize(Policy = "Permission.Content.Edit")]
public IActionResult EditContent() { }
```

### Password Complexity

Passwords are validated with customizable complexity requirements:

```csharp
// Password requirements are built into UserBase and form models
// - Minimum 6 characters, maximum 40 characters
// - At least 1 uppercase letter
// - At least 1 lowercase letter
// - At least 1 digit
```

### Localized Validation

All validation messages support localization through resource files:

```csharp
// Messages automatically use the configured culture
[Required(ErrorMessageResourceName = nameof(Messages.XRequired),
         ErrorMessageResourceType = typeof(Messages))]
public string Email { get; set; }
```

---

## 🛠️ Development

### Building the Solution

```bash
# Build all projects
dotnet build NuvTools.Security.Identity.slnx

# Build in Release mode (generates NuGet packages)
dotnet build NuvTools.Security.Identity.slnx -c Release

# Clean build artifacts
dotnet clean NuvTools.Security.Identity.slnx
```

### Project Structure

```
nuvtools-security-identity/
├── src/
│   ├── NuvTools.Security.Identity.Models/
│   ├── NuvTools.Security.Identity/
│   ├── NuvTools.Security.Identity.AspNetCore/
│   └── NuvTools.Security.Identity.EntityFrameworkCore/
├── CLAUDE.md                    # Developer guidance for AI assistants
├── README.md                    # This file
└── NuvTools.Security.Identity.slnx
```

---

## 📄 License

This project is licensed under the terms specified in the [LICENSE](LICENSE) file.

---

## 🔗 Links

- **Website:** [https://nuvtools.com](https://nuvtools.com)
- **GitHub Repository:** [https://github.com/nuvtools/nuvtools-security-identity](https://github.com/nuvtools/nuvtools-security-identity)
- **NuGet Packages:** [https://www.nuget.org/profiles/NuvTools](https://www.nuget.org/profiles/NuvTools)

---

## 📝 Version

Current Version: **10.0.0**