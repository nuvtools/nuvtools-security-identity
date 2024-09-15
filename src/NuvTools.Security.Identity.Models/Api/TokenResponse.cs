namespace NuvTools.Security.Identity.Models.Api;
public class TokenResponse
{
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}