using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using NuvTools.Notification.Mail;
using NuvTools.Common.ResultWrapper;
using NuvTools.Resources;
using NuvTools.Security.Identity.Models;
using NuvTools.Security.Identity.Models.Form;
using System.Text;
using System.Text.Encodings.Web;

namespace NuvTools.Security.Identity.AspNetCore.Services;

public abstract class UserServiceBase<TUser, TRole, TKey> where TUser : UserBase<TKey>
                                           where TRole : IdentityRole<TKey>
                                           where TKey : IEquatable<TKey>
{
    protected readonly UserManager<TUser> _userManager;
    protected readonly RoleManager<TRole> _roleManager;
    protected readonly IMailService _mailService;

    private const string USER_NOT_FOUND = "User not found.";

    public UserServiceBase(UserManager<TUser> userManager, RoleManager<TRole> roleManager, IMailService mailService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _mailService = mailService;
    }

    public IQueryable<TUser> GetAllAsync()
    {
        return _userManager.Users;
    }

    public async Task<TUser?> GetAsync(TKey id)
    {
        return await _userManager.FindByIdAsync(id.ToString()!);
    }

    public async Task<TUser?> GetByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<IResult> DeleteAsync(TKey id)
    {
        var user = await GetAsync(id);

        if (user == null)
            return Result.ValidationFail(Messages.UserNotFound);

        var result = await _userManager.DeleteAsync(user);

        return result.Succeeded ?
                Result.Success(Messages.OperationPerformedSuccessfully) :
                Result<string>.Fail(Messages.TheOperationCouldNotBePerformed);
    }

    public async Task<IResult<TKey>> CreateAsync(TUser value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value.Email, nameof(value.Email));
        ArgumentException.ThrowIfNullOrEmpty(value.Password, nameof(value.Password));

        var userWithSameEmail = await _userManager.FindByEmailAsync(value.Email);
        if (userWithSameEmail != null)
            return Result<TKey>.ValidationFail(string.Format(DynamicValidationMessages.EmailXAlreadyTaken, value.Email));

        value.UserName = value.Email;

        var result = await _userManager.CreateAsync(value, value.Password);

        if (!result.Succeeded)
            return Result<TKey>.Fail(result.Errors.Select(a => a.Description).ToList());

        return Result<TKey>.Success(value.Id);
    }

    public async Task<IResult<TKey>> CreateWithRolesAsync(TUser value, params string[] roles)
    {
        var result = await CreateAsync(value);

        if (!result.Succeeded)
            return result;

        var resultRoles = await _userManager.AddToRolesAsync(value, roles);

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

        var result = await _userManager.UpdateAsync(user);

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
        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded ?
                Result.Success(Messages.OperationPerformedSuccessfully) :
                Result<string>.Fail(Messages.TheOperationCouldNotBePerformed);
    }

    #region E-mail Confirmation

    public async Task<IResult<TKey>> CreateAndSendConfirmationEmailAsync(TUser value, string origin, string from, string? message = null, string? subject = null, params string[] roles)
    {
        var result = await CreateWithRolesAsync(value, roles);

        if (!result.Succeeded)
            return result;

        if (value.EmailConfirmed)
            return Result<TKey>.Success(value.Id, message: Messages.OperationPerformedSuccessfully);

        return await SendConfirmationEmailAsync(value, origin, from, message, subject);
    }

    public async Task<IResult<TKey>> SendConfirmationEmailAsync(TUser value, string origin, string from, string? message = null, string? subject = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(value.Email, nameof(value.Email));

        var verificationUri = await GenerateEmailConfirmationUriAsync(value, origin);
        await _mailService.SendAsync(new MailMessage()
        {
            From = new MailAddress { Address = from },
            To = [new() { Address = value.Email, DisplayName = value.Name }],
            Body = string.Format(message ?? DynamicMessages.EmailConfirmationContentWithLinkX, HtmlEncoder.Default.Encode(verificationUri)),
            Subject = subject ?? Messages.ConfirmRegistration
        });

        return Result<TKey>.Success(value.Id, message: Messages.OperationPerformedSuccessfully);
    }

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
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(value);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        return token;
    }

    #endregion

    #region Create User

    public async Task<IResult> ConfirmEmailAsync(TKey id, string token)
    {
        var user = await _userManager.FindByIdAsync(id.ToString()!);

        if (user is null)
            return Result.Fail(USER_NOT_FOUND);

        token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

        user.Status = true;

        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded ?
                Result.Success(Messages.EmailConfirmed) :
                Result.Fail(Messages.TheOperationCouldNotBePerformed);
    }

    #endregion

    #region Change E-mail

    public async Task<IResult> RequestChangeEmailAsync(string email, string newEmail, string origin, string from, string? message = null, string? subject = null, string route = "security/change-email")
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
        {
            return Result.ValidationFail(Messages.TheOperationCouldNotBePerformed);
        }

        var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var _enpointUri = new Uri(string.Concat($"{origin}/", route));
        var pageUrl = QueryHelpers.AddQueryString(_enpointUri.ToString(), "Token", token);
        var request = new MailMessage()
        {
            From = new MailAddress { Address = from },
            To = new List<MailAddress> { new() { Address = email } },
            Body = string.Format(message ?? "Please change your e-mail by <a href='{0}'>clicking here</a>", HtmlEncoder.Default.Encode(pageUrl)),
            Subject = subject ?? "Change Email",
        };
        await _mailService.SendAsync(request);

        return Result.Success(Messages.PasswordResetMailWasSent);
    }

    public async Task<IResult> ChangeEmailAsync(TKey id, UpdateProfileForm value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value.Email, nameof(value.Email));
        ArgumentException.ThrowIfNullOrEmpty(value.Token, nameof(value.Token));

        var user = await _userManager.FindByIdAsync(id.ToString()!);
        if (user is null)
            return Result.Fail(USER_NOT_FOUND);

        var identityResult = await _userManager.ChangeEmailAsync(
            user,
            value.Email,
            value.Token);
        var errors = identityResult.Errors.Select(e => e.Description).ToList();
        return identityResult.Succeeded ? Result.Success() : Result.Fail(errors);
    }

    #endregion

    #region Change Password

    public async Task<IResult> ChangePasswordAsync(TKey id, ChangePasswordForm value)
    {
        if (value.Password is null || value.NewPassword is null)
            throw new ArgumentNullException($"{nameof(value.Password)} or {nameof(value.NewPassword)} is empty.");

        var user = await _userManager.FindByIdAsync(id.ToString()!);
        if (user == null)
            return Result.Fail(USER_NOT_FOUND);

        var identityResult = await _userManager.ChangePasswordAsync(
            user,
            value.Password,
            value.NewPassword);
        var errors = identityResult.Errors.Select(e => e.Description).ToList();
        return identityResult.Succeeded ? Result.Success() : Result.Fail(errors);
    }

    #endregion

    #region Reset or Forgot Password

    public async Task<IResult> RequestResetPasswordAsync(string from, string email, string origin, string? message = null, string? subject = null, string route = "security/reset-password")
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null || !await _userManager.IsEmailConfirmedAsync(user))
            return Result.ValidationFail(Messages.TheOperationCouldNotBePerformed);

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var _enpointUri = new Uri(string.Concat($"{origin}/", route));
        var passwordResetURL = QueryHelpers.AddQueryString(_enpointUri.ToString(), "Token", token);
        var request = new MailMessage()
        {
            From = new MailAddress { Address = from },
            Body = string.Format(message ?? DynamicMessages.PasswordResetEmailContentWithLinkX, HtmlEncoder.Default.Encode(passwordResetURL)),
            Subject = subject ?? Fields.ResetPassword,
            To = [new() { Address = email }]
        };
        await _mailService.SendAsync(request);

        return Result.Success(Messages.PasswordResetMailWasSent);
    }

    public async Task<IResult> ResetPasswordAsync(ResetPasswordForm value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value.Email, nameof(value.Email));
        ArgumentException.ThrowIfNullOrEmpty(value.Password, nameof(value.Password));
        ArgumentException.ThrowIfNullOrEmpty(value.Token, nameof(value.Token));

        var user = await _userManager.FindByEmailAsync(value.Email);
        if (user == null)
            return Result.ValidationFail(Messages.UserNotFound);

        var result = await _userManager.ResetPasswordAsync(user, Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(value.Token)), value.Password);

        return result.Succeeded ?
             Result.Success(Messages.PasswordResetSuccessful) :
             Result.Fail(Messages.TheOperationCouldNotBePerformed);
    }

    #endregion

    #region Roles

    public async Task<IList<string>?> GetRolesAsync(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null) return null;

        return await _userManager.GetRolesAsync(user);
    }

    public async Task<IResult> UpdateRolesAsync(UserRoles value)
    {
        var user = await _userManager.FindByIdAsync(value.UserId.ToString());

        if (user is null)
            return Result.Fail();

        var roles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, roles);

        if (value.Roles != null)
            await _userManager.AddToRolesAsync(user, value.Roles);

        return Result.Success(Messages.OperationPerformedSuccessfully);
    }

    #endregion
}