using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using NuvTools.Common.ResultWrapper;
using NuvTools.Resources;
using NuvTools.Security.Identity.Models;
using System.Text;

namespace NuvTools.Security.Identity.AspNetCore.Services;

/// <summary>
/// Provides a base service for user management operations, built upon <see cref="UserManager{TUser}"/>.
/// </summary>
/// <typeparam name="TUser">The user entity type.</typeparam>
/// <typeparam name="TRole">The role entity type.</typeparam>
/// <typeparam name="TKey">The type of the user's primary key.</typeparam>
/// <remarks>
/// This abstract base class consolidates common user-related actions such as creation, updates,
/// password management, email confirmation, and role assignments.  
/// It standardizes the return model using <see cref="IResult"/> and <see cref="IResult{T}"/>.
/// </remarks>
public abstract class UserServiceBase<TUser, TRole, TKey>(
    UserManager<TUser> userManager)
    where TUser : UserBase<TKey>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
{
    private const string UserNotFoundMessage = "User not found.";

    #region Retrieval

    /// <summary>
    /// Gets all users as an <see cref="IQueryable{T}"/> queryable collection.
    /// </summary>
    public IQueryable<TUser> GetAllAsync() => userManager.Users;

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    public async Task<TUser?> GetAsync(TKey id) =>
        await userManager.FindByIdAsync(id.ToString()!);

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    public async Task<TUser?> GetByEmailAsync(string email) =>
        await userManager.FindByEmailAsync(email);

    #endregion

    #region Create / Delete / Update

    /// <summary>
    /// Deletes a user by identifier.
    /// </summary>
    public async Task<IResult> DeleteAsync(TKey id)
    {
        var user = await GetAsync(id);
        if (user is null)
            return Result.ValidationFail(Messages.UserNotFound);

        var result = await userManager.DeleteAsync(user);

        return result.Succeeded
            ? Result.Success(Messages.OperationPerformedSuccessfully)
            : Result.Fail(Messages.TheOperationCouldNotBePerformed);
    }

    /// <summary>
    /// Creates a new user in the system.
    /// </summary>
    public async Task<IResult<TKey>> CreateAsync(TUser value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value.Email, nameof(value.Email));
        ArgumentException.ThrowIfNullOrEmpty(value.Password, nameof(value.Password));

        var existing = await userManager.FindByEmailAsync(value.Email);
        if (existing is not null)
            return Result<TKey>.ValidationFail(
                string.Format(Messages.EmailXAlreadyTaken, value.Email));

        value.UserName = value.Email;

        var result = await userManager.CreateAsync(value, value.Password);

        if (!result.Succeeded)
            return Result<TKey>.Fail(result.Errors.Select(e => e.Description).ToList());

        return Result<TKey>.Success(value.Id, Messages.OperationPerformedSuccessfully);
    }

    /// <summary>
    /// Creates a new user and assigns one or more roles.
    /// </summary>
    public async Task<IResult<TKey>> CreateWithRolesAsync(TUser value, params string[] roles)
    {
        var creationResult = await CreateAsync(value);
        if (!creationResult.Succeeded)
            return creationResult;

        var roleResult = await userManager.AddToRolesAsync(value, roles);

        return roleResult.Succeeded
            ? Result<TKey>.Success(creationResult.Data, Messages.OperationPerformedSuccessfully)
            : Result<TKey>.Fail(roleResult.Errors.Select(e => e.Description).ToList());
    }

    /// <summary>
    /// Updates basic user information (name, surname, etc.).
    /// </summary>
    public async Task<IResult> UpdateAsync(TUser value)
    {
        var user = await GetAsync(value.Id) ?? await GetByEmailAsync(value.Email ?? string.Empty);
        if (user is null)
            return Result.ValidationFail(Messages.UserNotFound);

        user.Name = value.Name;
        user.Surname = value.Surname;

        var result = await userManager.UpdateAsync(user);
        return result.Succeeded
            ? Result.Success(Messages.OperationPerformedSuccessfully)
            : Result.Fail(Messages.TheOperationCouldNotBePerformed);
    }

    /// <summary>
    /// Toggles the user's active status.
    /// </summary>
    public virtual async Task<IResult> ToggleUserStatusAsync(TKey id)
    {
        var user = await GetAsync(id);
        if (user is null)
            return Result.ValidationFail(Messages.UserNotFound);

        user.Status = !user.Status;
        var result = await userManager.UpdateAsync(user);

        return result.Succeeded
            ? Result.Success(Messages.OperationPerformedSuccessfully)
            : Result.Fail(Messages.TheOperationCouldNotBePerformed);
    }

    #endregion

    #region Email Confirmation

    /// <summary>
    /// Generates a verification URL for confirming the user's email.
    /// </summary>
    public async Task<string> GenerateEmailConfirmationUriAsync(TUser user, string origin, string route = "login")
    {
        var token = await GenerateEmailConfirmationTokenAsync(user);
        var endpointUri = new Uri($"{origin}/{route}");
        var verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", user.Id.ToString()!);
        return QueryHelpers.AddQueryString(verificationUri, "code", token);
    }

    /// <summary>
    /// Generates a base64-encoded token used for email confirmation.
    /// </summary>
    public async Task<string> GenerateEmailConfirmationTokenAsync(TUser user)
    {
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
    }

    /// <summary>
    /// Confirms a user's email address.
    /// </summary>
    public async Task<IResult> ConfirmEmailAsync(TKey id, string token)
    {
        var user = await userManager.FindByIdAsync(id.ToString()!);
        if (user is null)
            return Result.Fail(UserNotFoundMessage);

        token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        user.Status = true;

        var result = await userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded
            ? Result.Success(Messages.EmailConfirmed)
            : Result.Fail(Messages.TheOperationCouldNotBePerformed);
    }

    #endregion

    #region Change Email

    /// <summary>
    /// Generates a URL for confirming an email change request.
    /// </summary>
    public async Task<IResult<string>> RequestChangeEmailUrlAsync(string email, string newEmail, string origin, string route = "security/change-email")
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null || !await userManager.IsEmailConfirmedAsync(user))
            return Result<string>.ValidationFail(Messages.TheOperationCouldNotBePerformed);

        var token = await userManager.GenerateChangeEmailTokenAsync(user, newEmail);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var endpointUri = new Uri($"{origin}/{route}");
        var url = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", token);
        return Result<string>.Success(url);
    }

    /// <summary>
    /// Applies an email change using a confirmation token.
    /// </summary>
    public async Task<IResult> ChangeEmailAsync(TKey id, string email, string token)
    {
        ArgumentException.ThrowIfNullOrEmpty(email, nameof(email));
        ArgumentException.ThrowIfNullOrEmpty(token, nameof(token));

        var user = await userManager.FindByIdAsync(id.ToString()!);
        if (user is null)
            return Result.Fail(UserNotFoundMessage);

        var result = await userManager.ChangeEmailAsync(user, email, token);
        var errors = result.Errors.Select(e => e.Description).ToList();
        return result.Succeeded ? Result.Success() : Result.Fail(errors);
    }

    #endregion

    #region Change Password

    /// <summary>
    /// Changes the user's password.
    /// </summary>
    public async Task<IResult> ChangePasswordAsync(TKey id, string password, string newPassword)
    {
        ArgumentException.ThrowIfNullOrEmpty(password, nameof(password));
        ArgumentException.ThrowIfNullOrEmpty(newPassword, nameof(newPassword));

        var user = await userManager.FindByIdAsync(id.ToString()!);
        if (user is null)
            return Result.Fail(UserNotFoundMessage);

        var result = await userManager.ChangePasswordAsync(user, password, newPassword);
        var errors = result.Errors.Select(e => e.Description).ToList();
        return result.Succeeded ? Result.Success() : Result.Fail(errors);
    }

    #endregion

    #region Reset / Forgot Password

    /// <summary>
    /// Generates a password reset URL for a user with a confirmed email.
    /// </summary>
    public async Task<IResult<string>> RequestResetPasswordUrlAsync(string email, string origin, string route = "security/reset-password")
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null || !await userManager.IsEmailConfirmedAsync(user))
            return Result<string>.ValidationFail(Messages.TheOperationCouldNotBePerformed);

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var endpointUri = new Uri($"{origin}/{route}");
        var resetUrl = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", token);
        return Result<string>.Success(resetUrl);
    }

    /// <summary>
    /// Resets the user's password using a reset token.
    /// </summary>
    public async Task<IResult> ResetPasswordAsync(string email, string newPassword, string token)
    {
        ArgumentException.ThrowIfNullOrEmpty(email, nameof(email));
        ArgumentException.ThrowIfNullOrEmpty(newPassword, nameof(newPassword));
        ArgumentException.ThrowIfNullOrEmpty(token, nameof(token));

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.ValidationFail(Messages.UserNotFound);

        var decoded = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        var result = await userManager.ResetPasswordAsync(user, decoded, newPassword);

        return result.Succeeded
            ? Result.Success(Messages.PasswordResetSuccessful)
            : Result.Fail(Messages.TheOperationCouldNotBePerformed);
    }

    #endregion

    #region Roles

    /// <summary>
    /// Retrieves all role names assigned to a user.
    /// </summary>
    public async Task<IList<string>?> GetRolesAsync(TKey id)
    {
        var user = await userManager.FindByIdAsync(id.ToString()!);
        return user is null ? null : await userManager.GetRolesAsync(user);
    }

    /// <summary>
    /// Replaces all roles of a user with a new set of roles.
    /// </summary>
    public async Task<IResult> UpdateRolesAsync(TKey id, IList<string> roles)
    {
        var user = await userManager.FindByIdAsync(id.ToString()!);
        if (user is null)
            return Result.Fail();

        var existingRoles = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, existingRoles);

        if (roles.Count > 0)
            await userManager.AddToRolesAsync(user, roles);

        return Result.Success(Messages.OperationPerformedSuccessfully);
    }

    #endregion
}