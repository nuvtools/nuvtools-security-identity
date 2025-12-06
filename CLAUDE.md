# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Nuv Tools Security is a modular library suite for ASP.NET Identity integration targeting .NET 8, 9, and 10. The solution consists of four independent NuGet packages that work together to provide identity models, helpers, ASP.NET Core integration, and Entity Framework Core persistence.

## Solution Structure

The solution contains four main projects under `src/`:

1. **NuvTools.Security.Identity.Models** - Base models and forms for ASP.NET Identity
   - Contains `UserBase<TKey>` and `RoleBase<TKey>` abstract classes
   - Includes form models (LoginForm, ChangePasswordForm, etc.)
   - API models (TokenResponse, RefreshTokenRequest)
   - Uses localized validation attributes via NuvTools.Resources

2. **NuvTools.Security.Identity** - Helper library with authorization utilities
   - Permission-based authorization: `AuthorizationPermissionPolicyProvider`, `PermissionRequirement`, `PermissionAuthorizationHandler`
   - Enables dynamic policy generation based on `ClaimTypes.Permission`

3. **NuvTools.Security.Identity.AspNetCore** - Server-side ASP.NET Core integration
   - `UserServiceBase<TUser, TRole, TKey>` - Comprehensive user management service
   - `RoleManagerExtensions` - Role management helpers
   - Standardized returns using `IResult` and `IResult<T>` from NuvTools.Common.ResultWrapper
   - Handles: user CRUD, email confirmation, password management, role assignments

4. **NuvTools.Security.Identity.EntityFrameworkCore** - EF Core persistence layer
   - `IdentityDbContextBase<TUser, TRole, TIdentityKey>` - Base context extending IdentityDbContext
   - Implements `IDbContextCommands` and `IDbContextWithListCommands` from NuvTools.Data.EntityFrameworkCore
   - Provides transaction support and batch operations (SyncFromListAsync, AddOrUpdateFromListAsync, etc.)

## Building and Testing

**Build the solution:**
```bash
dotnet build NuvTools.Security.Identity.slnx
```

**Build in Release mode (generates NuGet packages):**
```bash
dotnet build NuvTools.Security.Identity.slnx -c Release
```

Note: `GeneratePackageOnBuild` is enabled in all projects, so building in Release mode creates NuGet packages.

**Clean build artifacts:**
```bash
dotnet clean NuvTools.Security.Identity.slnx
```

**Build a specific project:**
```bash
dotnet build src/NuvTools.Security.Identity.Models/NuvTools.Security.Identity.Models.csproj
```

## Project Dependencies

### External NuvTools Dependencies
All projects depend on other NuvTools packages (version 10.0.0):
- NuvTools.Resources (localized validation messages)
- NuvTools.Validation (password complexity attributes)
- NuvTools.Common (IResult wrappers)
- NuvTools.Security (ClaimTypes, security models)
- NuvTools.Data.EntityFrameworkCore (DbContext extensions)

### Microsoft Dependencies
- Microsoft.AspNetCore.Identity.EntityFrameworkCore (version varies by target framework)
- Microsoft.Extensions.Identity.Stores
- Microsoft.AspNetCore.Authorization

### Version-Specific Package References
The EntityFrameworkCore project uses conditional package references:
- .NET 10: Microsoft.AspNetCore.Identity.EntityFrameworkCore 10.0.0
- .NET 9: Microsoft.AspNetCore.Identity.EntityFrameworkCore 9.0.11
- .NET 8: Microsoft.AspNetCore.Identity.EntityFrameworkCore 8.0.22

## Architecture Patterns

### Generic Type Parameters
All core classes use generic type parameters for flexibility:
- `TUser` - User entity type (must inherit from `UserBase<TKey>` or `IdentityUser<TKey>`)
- `TRole` - Role entity type (must inherit from `IdentityRole<TKey>`)
- `TKey` - Primary key type (e.g., Guid, int) with `IEquatable<TKey>` constraint

### Multi-Targeting
All projects target three frameworks: `net8`, `net9`, and `net10.0`. When adding features, ensure compatibility across all targets.

### Localization
This codebase uses resource-based localization extensively:
- Display attributes reference `Fields` resource type
- Validation messages reference `DynamicValidationMessages` and `Messages` resources
- All error messages should use localized resources, not hardcoded strings

### Result Pattern
The AspNetCore library uses `IResult` and `IResult<T>` wrappers instead of throwing exceptions:
- `Result.Success()` / `Result<T>.Success(data)`
- `Result.Fail(message)` / `Result.ValidationFail(message)`
- Check `result.Succeeded` before accessing `result.Data`

### NotMapped Properties
`UserBase<TKey>` includes unmapped properties for runtime use:
- `Password` - For registration/password change (not persisted)
- `RefreshToken` and `RefreshTokenExpiryTime` - For JWT refresh flows

## Code Conventions

### Validation Attributes
When adding validation to models:
- Use `[Display(Name = nameof(Fields.PropertyName), ResourceType = typeof(Fields))]`
- Use resource-based error messages: `ErrorMessageResourceName` and `ErrorMessageResourceType`
- Password complexity uses NuvTools.Validation attributes: `[PasswordComplexityCapitalLetters(1)]`, `[PasswordComplexityLowerCaseLetters(1)]`, `[PasswordComplexityDigits(1)]`

### Service Method Patterns
When extending `UserServiceBase`:
- Validate inputs with `ArgumentException.ThrowIfNullOrEmpty()`
- Return `IResult` or `IResult<T>` (never throw exceptions for business logic failures)
- Use localized messages from `Messages` resources
- Check user existence before operations: `await GetAsync(id)` or `await GetByEmailAsync(email)`

### Entity Framework Extensions
The `IdentityDbContextBase` exposes batch operations from NuvTools.Data.EntityFrameworkCore:
- `SyncFromListAsync` - Synchronizes database with a list (adds new, removes missing)
- `AddOrUpdateFromListAsync` - Upserts entities from a list
- `AddOrRemoveFromListAsync` - Adds or removes based on list presence

## Package Metadata

All projects share common NuGet metadata:
- Authors: Nuv Tools
- License: LICENSE file in repository root
- Icon: icon.png in repository root
- Repository: https://github.com/nuvtools/nuvtools-security-identity
- Website: https://nuvtools.com
- Version: 10.0.0

When updating package versions, update all four .csproj files consistently.
