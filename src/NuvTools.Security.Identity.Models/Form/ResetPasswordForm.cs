namespace NuvTools.Security.Identity.Models.Form;

/// <summary>
/// Represents the form data for resetting a user's password using a reset token.
/// </summary>
/// <remarks>
/// This form is used to complete the password reset process after a user has requested
/// a password reset via <see cref="ForgotPasswordForm"/>.
/// The token is typically sent via email and must be provided along with the new password.
/// </remarks>
public class ResetPasswordForm
{
    /// <summary>
    /// Gets or sets the email address of the user resetting their password.
    /// </summary>
    /// <remarks>
    /// This email is used to identify the user account that will have its password reset.
    /// </remarks>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the new password the user wants to set.
    /// </summary>
    /// <remarks>
    /// This will become the user's new password after successful validation and token verification.
    /// </remarks>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the password reset token that was sent to the user's email.
    /// </summary>
    /// <remarks>
    /// This token must be valid and not expired for the password reset to succeed.
    /// The token is typically Base64-URL encoded.
    /// </remarks>
    public string? Token { get; set; }
}