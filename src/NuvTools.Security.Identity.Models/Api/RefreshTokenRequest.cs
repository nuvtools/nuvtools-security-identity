namespace NuvTools.Security.Identity.Models.Api;

public class RefreshTokenRequest
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}