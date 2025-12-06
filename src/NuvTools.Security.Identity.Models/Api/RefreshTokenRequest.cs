namespace NuvTools.Security.Identity.Models.Api;

/// <summary>
/// Represents the request for refreshing an expired access token.
/// </summary>
/// <remarks>
/// This request is used to obtain a new access token without requiring the user to re-authenticate.
/// Both the expired access token and a valid refresh token must be provided.
/// </remarks>
public class RefreshTokenRequest
{
    /// <summary>
    /// Gets or sets the expired or soon-to-expire JWT access token.
    /// </summary>
    /// <remarks>
    /// This token may be expired but is still needed to validate the refresh request
    /// and extract user information for generating a new token.
    /// </remarks>
    public string? Token { get; set; }

    /// <summary>
    /// Gets or sets the refresh token issued during the original authentication.
    /// </summary>
    /// <remarks>
    /// This token must be valid and not expired. It is used to verify that the refresh request
    /// is legitimate and to generate a new access token and refresh token pair.
    /// </remarks>
    public string? RefreshToken { get; set; }
}