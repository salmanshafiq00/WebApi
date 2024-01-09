namespace WebApi.Application.Common.Models;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int TokenLimit { get; set; }
    public string TokenType { get; set; } = string.Empty;
}
