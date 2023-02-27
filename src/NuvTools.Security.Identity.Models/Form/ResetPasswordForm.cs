namespace NuvTools.Security.Identity.Models.Form;
public class ResetPasswordForm
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Token { get; set; }
}