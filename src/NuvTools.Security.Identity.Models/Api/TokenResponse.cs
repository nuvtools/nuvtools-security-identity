namespace NuvTools.Security.Identity.Models.Api;

/// <summary>
/// Represents the response containing authentication tokens returned after successful login.
/// </summary>
/// <remarks>
/// This response typically includes a JWT access token, a refresh token, and the expiry time
/// for the refresh token. It is commonly used in API authentication scenarios.
/// </remarks>
public class TokenResponse
{
    /// <summary>
    /// Gets or sets the JWT access token.
    /// </summary>
    /// <remarks>
    /// This token is used to authenticate API requests and typically has a short lifespan.
    /// It should be included in the Authorization header as a Bearer token for authenticated requests.
    /// </remarks>
    public string? Token { get; set; }

    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    /// <remarks>
    /// This token is used to obtain a new access token when the current one expires.
    /// Refresh tokens typically have a longer lifespan than access tokens and should be stored securely.
    /// </remarks>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets the expiry date and time for the refresh token.
    /// </summary>
    /// <remarks>
    /// After this time, the refresh token is no longer valid and the user must re-authenticate.
    /// </remarks>
    public DateTime? RefreshTokenExpiryTime { get; set; }
}