using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using NuvTools.Common.ResultWrapper;
using NuvTools.Resources;
using NuvTools.Security.Identity.Models;
using System.Text;

namespace NuvTools.Security.Identity.AspNetCore.Services;

public abstract class UserServiceBase<TUser, TRole, TKey>(
                            UserManager<TUser> userManager) where TUser : UserBase<TKey>
                                                        where TRole : IdentityRole<TKey>
                                                        where TKey : IEquatable<TKey>
{
    private const string UserNotFound = "User not found.";

    public IQueryable<TUser> GetAllAsync()
    {
        return userManager.Users;
    }

    public async Task<TUser?> GetAsync(TKey id)
    {
        return await userManager.FindByIdAsync(id.ToString()!);
    }

    public async Task<TUser?> GetByEmailAsync(string email)
    {
        return await userManager.FindByEmailAsync(email);
    }

    public async Task<IResult> DeleteAsync(TKey id)
    {
        var user = await GetAsync(id);

        if (user == null)
            return Result.ValidationFail(Messages.UserNotFound);

        var result = await userManager.DeleteAsync(user);

        return result.Succeeded ?
                Result.Success(Messages.OperationPerformedSuccessfully) :
                Result<string>.Fail(Messages.TheOperationCouldNotBePerformed);
    }

    public async Task<IResult<TKey>> CreateAsync(TUser value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value.Email, nameof(value.Email));
        ArgumentException.ThrowIfNullOrEmpty(value.Password, nameof(value.Password));

        var userWithSameEmail = await userManager.FindByEmailAsync(value.Email);
        if (userWithSameEmail != null)
            return Result<TKey>.ValidationFail(string.Format(DynamicValidationMessages.EmailXAlreadyTaken, value.Email));

        value.UserName = value.Email;

        var result = await userManager.CreateAsync(value, value.Password);

        if (!result.Succeeded)
            return Result<TKey>.Fail(result.Errors.Select(a => a.Description).ToList());

        return Result<TKey>.Success(value.Id);
    }

    public async Task<IResult<TKey>> CreateWithRolesAsync(TUser value, params string[] roles)
    {
        var result = await CreateAsync(value);

        if (!result.Succeeded)
            return result;

        var resultRoles = await userManager.AddToRolesAsync(value, roles);

        if (!resultRoles.Succeeded)
            return Result<TKey>.Fail(resultRoles.Errors.Select(a => a.Description).ToList());

        return Result<TKey>.Success(result.Data, message: Messages.OperationPerformedSuccessfully);
    }

    public async Task<IResult> UpdateAsync(TUser value)
    {
        var user = await GetAsync(value.Id);

        if (user == null && !string.IsNullOrEmpty(value.Email))
            user = await GetByEmailAsync(value.Email);

        if (user == null)
            return Result.ValidationFail(Messages.UserNotFound);

        user.Name = value.Name;
        user.Surname = value.Surname;

        var result = await userManager.UpdateAsync(user);

        return result.Succeeded ?
                Result.Success(Messages.OperationPerformedSuccessfully) :
                Result.Fail(Messages.TheOperationCouldNotBePerformed);
    }

    public virtual async Task<IResult> ToggleUserStatusAsync(TKey id)
    {
        var user = await GetAsync(id);

        if (user == null)
            return Result.ValidationFail(Messages.UserNotFound);

        user.Status = !user.Status;
        var result = await userManager.UpdateAsync(user);

        return result.Succeeded ?
                Result.Success(Messages.OperationPerformedSuccessfully) :
                Result<string>.Fail(Messages.TheOperationCouldNotBePerformed);
    }

    #region E-mail Confirmation

    public async Task<string> GenerateEmailConfirmationUriAsync(TUser value, string origin, string route = "login")
    {
        string token = await GenerateEmailConfirmationTokenAsync(value);
        var _enpointUri = new Uri(string.Concat($"{origin}/", route));
        var verificationUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "userId", value.Id.ToString()!);
        verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", token);
        return verificationUri;
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(TUser value)
    {
        var token = await userManager.GenerateEmailConfirmationTokenAsync(value);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        return token;
    }

    #endregion

    #region Create User

    public async Task<IResult> ConfirmEmailAsync(TKey id, string token)
    {
        var user = await userManager.FindByIdAsync(id.ToString()!);

        if (user is null)
            return Result.Fail(UserNotFound);

        token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

        user.Status = true;

        var result = await userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded ?
                Result.Success(Messages.EmailConfirmed) :
                Result.Fail(Messages.TheOperationCouldNotBePerformed);
    }

    #endregion

    #region Change E-mail

    public async Task<IResult<string>> RequestChangeEmailUrlAsync(string email, string newEmail, string origin, string route = "security/change-email")
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null || !await userManager.IsEmailConfirmedAsync(user))
        {
            return Result<string>.ValidationFail(Messages.TheOperationCouldNotBePerformed);
        }

        var token = await userManager.GenerateChangeEmailTokenAsync(user, newEmail);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var _endpointUri = new Uri(string.Concat($"{origin}/", route));
        var pageUrl = QueryHelpers.AddQueryString(_endpointUri.ToString(), "Token", token);

        return Result<string>.Success(pageUrl);
    }

    public async Task<IResult> ChangeEmailAsync(TKey id, string email, string token)
    {
        ArgumentException.ThrowIfNullOrEmpty(email, nameof(email));
        ArgumentException.ThrowIfNullOrEmpty(token, nameof(token));

        var user = await userManager.FindByIdAsync(id.ToString()!);
        if (user is null)
            return Result.Fail(UserNotFound);

        var identityResult = await userManager.ChangeEmailAsync(user, email, token);
        var errors = identityResult.Errors.Select(e => e.Description).ToList();
        return identityResult.Succeeded ? Result.Success() : Result.Fail(errors);
    }

    #endregion

    #region Change Password

    public async Task<IResult> ChangePasswordAsync(TKey id, string password, string newPassword)
    {
        ArgumentException.ThrowIfNullOrEmpty(password, nameof(password));
        ArgumentException.ThrowIfNullOrEmpty(newPassword, nameof(newPassword));

        var user = await userManager.FindByIdAsync(id.ToString()!);
        if (user == null)
            return Result.Fail(UserNotFound);

        var identityResult = await userManager.ChangePasswordAsync(
            user,
            password,
            newPassword);
        var errors = identityResult.Errors.Select(e => e.Description).ToList();
        return identityResult.Succeeded ? Result.Success() : Result.Fail(errors);
    }

    #endregion

    #region Reset or Forgot Password

    public async Task<IResult<string>> RequestResetPasswordUrlAsync(string email, string origin, string route = "security/reset-password")
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null || !await userManager.IsEmailConfirmedAsync(user))
            return Result<string>.ValidationFail(Messages.TheOperationCouldNotBePerformed);

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var _enpointUri = new Uri(string.Concat($"{origin}/", route));
        var passwordResetURL = QueryHelpers.AddQueryString(_enpointUri.ToString(), "Token", token);

        return Result<string>.Success(passwordResetURL);
    }

    public async Task<IResult> ResetPasswordAsync(string email, string newPassword, string token)
    {
        ArgumentException.ThrowIfNullOrEmpty(email, nameof(email));
        ArgumentException.ThrowIfNullOrEmpty(newPassword, nameof(newPassword));
        ArgumentException.ThrowIfNullOrEmpty(token, nameof(token));

        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
            return Result.ValidationFail(Messages.UserNotFound);

        var result = await userManager.ResetPasswordAsync(user, Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token)), newPassword);

        return result.Succeeded ?
             Result.Success(Messages.PasswordResetSuccessful) :
             Result.Fail(Messages.TheOperationCouldNotBePerformed);
    }

    #endregion

    #region Roles

    public async Task<IList<string>?> GetRolesAsync(TKey id)
    {
        var user = await userManager.FindByIdAsync(id.ToString()!);

        if (user is null) return null;

        return await userManager.GetRolesAsync(user);
    }

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