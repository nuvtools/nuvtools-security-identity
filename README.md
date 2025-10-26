# Nuv Tools Security - ASP.NET Identity Libraries

Nuv Tools Security provides a modular set of libraries designed to simplify and enhance ASP.NET Identity integration for modern .NET applications. These libraries target .NET 8 and .NET 9, offering solutions for identity models, helper utilities, ASP.NET Core integration, and Entity Framework Core support.

---

## Overview

Nuv Tools Security is organized into four main libraries:

- **NuvTools.Security.Identity.Models**: Contains reusable models for ASP.NET Identity.
- **NuvTools.Security.Identity**: Offers helper methods and utilities to streamline identity management.
- **NuvTools.Security.Identity.AspNetCore**: Integrates ASP.NET Identity modules into server-side ASP.NET Core projects.
- **NuvTools.Security.Identity.EntityFrameworkCore**: Provides Entity Framework Core extensions for identity persistence.

Each library is designed for modular use, allowing you to include only the components relevant to your project.

---

## Libraries

### 1. NuvTools.Security.Identity.Models
- **Purpose:** Provides models for use with ASP.NET Identity modules.
- **Target Frameworks:** .NET 8, .NET 9
- **NuGet Package:** [NuvTools.Security.Identity.Models](https://www.nuget.org/packages/NuvTools.Security.Identity.Models)
- **Usage Example:**
    ```csharp
    using NuvTools.Security.Identity.Models;
    // Use identity models in your authentication logic
    ```

### 2. NuvTools.Security.Identity
- **Purpose:** Helper library for ASP.NET Identity, offering utilities and extensions.
- **Target Frameworks:** .NET 8, .NET 9
- **NuGet Package:** [NuvTools.Security.Identity](https://www.nuget.org/packages/NuvTools.Security.Identity)
- **Usage Example:**
    ```csharp
    using NuvTools.Security.Identity;
    // Use helper methods to simplify identity management
    ```

### 3. NuvTools.Security.Identity.AspNetCore
- **Purpose:** Server-side library for ASP.NET Core projects using Identity modules.
- **Target Frameworks:** .NET 8, .NET 9
- **NuGet Package:** [NuvTools.Security.Identity.AspNetCore](https://www.nuget.org/packages/NuvTools.Security.Identity.AspNetCore)
- **Usage Example:**
    ```csharp
    using NuvTools.Security.Identity.AspNetCore;
    // Integrate with ASP.NET Core Identity in your web application
    ```

### 4. NuvTools.Security.Identity.EntityFrameworkCore
- **Purpose:** Entity Framework Core helpers for ASP.NET Identity modules.
- **Target Frameworks:** .NET 8, .NET 9
- **NuGet Package:** [NuvTools.Security.Identity.EntityFrameworkCore](https://www.nuget.org/packages/NuvTools.Security.Identity.EntityFrameworkCore)
- **Usage Example:**
    ```csharp
    using NuvTools.Security.Identity.EntityFrameworkCore;
    // Use EF Core extensions for identity persistence
    ```