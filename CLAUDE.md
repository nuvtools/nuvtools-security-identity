# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

NuvTools.Security.Identity is a modular library suite for ASP.NET Identity integration targeting .NET 8, .NET 9, and .NET 10. The solution consists of four NuGet packages that provide identity models, permission-based authorization, user management services, and Entity Framework Core persistence.

- **src/NuvTools.Security.Identity.Models** - Base models, form DTOs, and API models with localized validation
- **src/NuvTools.Security.Identity** - Permission-based authorization with dynamic policy provider
- **src/NuvTools.Security.Identity.AspNetCore** - User management service, email/password workflows, and role extensions
- **src/NuvTools.Security.Identity.EntityFrameworkCore** - Identity DbContext with transaction management and batch operations

All libraries are published as NuGet packages.

## Build and Test Commands

**Note:** This solution uses the modern `.slnx` (XML-based solution) format introduced in Visual Studio 2022 v17.11.

### Build the solution
```bash
dotnet build NuvTools.Security.Identity.slnx
```

### Build for specific configuration
```bash
dotnet build NuvTools.Security.Identity.slnx --configuration Release
```

## Architecture and Key Components

### NuvTools.Security.Identity.Models Library

Base models and forms for ASP.NET Identity.

**UserBase\<TKey\>** - Abstract user entity extending IdentityUser\<TKey\>:
- Properties: Email (with validation), Name, Surname, Status
- NotMapped properties: Password, RefreshToken, RefreshTokenExpiryTime (runtime use only)
- Password validation: 6-40 chars, min 1 uppercase, 1 lowercase, 1 digit

**RoleBase\<TKey\>** - Abstract role entity extending IdentityRole\<TKey\>:
- Properties: Name (max 30 chars), Claims (NotMapped)

**Form Models** (all use localized validation from NuvTools.Resources):
- `LoginForm` - Email, Password
- `UserForm` - Id, Email, Name, Surname, Status, EmailConfirmed
- `UserWithPasswordForm` - Extends UserForm with Password/ConfirmPassword
- `ChangePasswordForm` - Current password, new password, confirm
- `ForgotPasswordForm` - Email
- `ResetPasswordForm` - Email, Password, Token
- `UpdateProfileForm` - Name, Surname, Email, Token

**API Models**: `TokenResponse`, `RefreshTokenRequest`

**UserRoles** - DTO for user-role associations (UserId, Roles)

### NuvTools.Security.Identity Library

Permission-based authorization system.

**AuthorizationPermissionPolicyProvider** - Custom IAuthorizationPolicyProvider:
- Dynamically generates policies for names starting with "Permission."
- Creates PermissionRequirement for matching policies
- Delegates non-permission policies to DefaultAuthorizationPolicyProvider

**PermissionRequirement** - IAuthorizationRequirement with permission string

**PermissionAuthorizationHandler** - AuthorizationHandler\<PermissionRequirement\>:
- Checks ClaimTypes.Permission claims (case-insensitive)
- Requires issuer = "LOCAL AUTHORITY"

### NuvTools.Security.Identity.AspNetCore Library

Server-side user management and role administration.

**UserServiceBase\<TUser, TRole, TKey\>** - Abstract service (all methods return IResult/IResult\<T\>):
- User CRUD: GetAllAsync, GetAsync, GetByEmailAsync, CreateAsync, UpdateAsync, DeleteAsync
- CreateWithRolesAsync - Create user and assign roles
- ToggleUserStatusAsync - Toggle Status flag
- Email confirmation: GenerateEmailConfirmationTokenAsync, GenerateEmailConfirmationUriAsync, ConfirmEmailAsync
- Email change: RequestChangeEmailUrlAsync, ChangeEmailAsync
- Password: ChangePasswordAsync, RequestResetPasswordUrlAsync, ResetPasswordAsync
- Roles: GetRolesAsync, UpdateRolesAsync

**RoleManagerExtensions** - Extension methods for RoleManager\<TRole\>:
- AddClaimsAsync, AddClaimAsync - Add claims to roles
- AddPermissionClaim, AddPermissionClaims - Add permission-type claims

### NuvTools.Security.Identity.EntityFrameworkCore Library

Identity persistence layer.

**IdentityDbContextBase\<TUser, TRole, TIdentityKey\>** - Abstract DbContext:
- Extends IdentityDbContext, implements IDbContextCommands and IDbContextWithListCommands
- Transactions: BeginTransactionAsync, CommitTransactionAsync, RollbackTransactionAsync
- Execution strategy: ExecuteWithStrategyAsync
- CRUD: AddAndSaveAsync, UpdateAndSaveAsync, RemoveAndSaveAsync
- Batch: SyncFromListAsync, AddOrUpdateFromListAsync, AddOrRemoveFromListAsync

## Architecture Patterns

### Generic Type Parameters
- `TUser` - User entity (inherits UserBase\<TKey\> or IdentityUser\<TKey\>)
- `TRole` - Role entity (inherits IdentityRole\<TKey\>)
- `TKey` - Primary key type (e.g., Guid, int) with IEquatable\<TKey\>

### Localization
All validation uses resource-based localization:
- Display attributes reference `Fields` resource type
- Validation messages reference `Messages` resources
- Error messages should use localized resources, not hardcoded strings

### Result Pattern
UserServiceBase uses IResult/IResult\<T\> wrappers from NuvTools.Common.ResultWrapper:
- `Result.Success()` / `Result<T>.Success(data)`
- `Result.Fail(message)` / `Result.ValidationFail(message)`

## Code Style and Conventions

- **Nullable reference types** are enabled (`<Nullable>enable</Nullable>`)
- **Implicit usings** are enabled (`<ImplicitUsings>enable</ImplicitUsings>`)
- **Code style enforcement** is enabled during build (`<EnforceCodeStyleInBuild>True</EnforceCodeStyleInBuild>`)
- **XML documentation generation** is required (`<GenerateDocumentationFile>True</GenerateDocumentationFile>`)

## Dependencies

### NuvTools.Security.Identity.Models
- Microsoft.Extensions.Identity.Stores [10.0.2,10.1.0)
- NuvTools.Resources [10.1.1,10.2.0)
- NuvTools.Validation [10.1.0,10.2.0)

### NuvTools.Security.Identity
- Microsoft.AspNetCore.Authorization [10.0.2,10.1.0)
- NuvTools.Security [10.1.0,10.2.0)

### NuvTools.Security.Identity.AspNetCore
- NuvTools.Common [10.0.2,10.1.0)
- NuvTools.Security [10.1.0,10.2.0)
- NuvTools.Security.Identity.Models (project reference)
- FrameworkReference: Microsoft.AspNetCore.App

### NuvTools.Security.Identity.EntityFrameworkCore
- Microsoft.AspNetCore.Identity.EntityFrameworkCore (version varies by target framework)
- NuvTools.Data.EntityFrameworkCore [10.1.0,10.2.0)
