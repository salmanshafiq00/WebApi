using Application.Common.Interfaces;
using Application.Constants;
using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using WebApi.Application.Common.Models;
using WebApi.Infrastructure.Identity;

namespace Infrastructure.Identity;
internal class AuthAccountService : IAuthAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtProvider _jwtProvider;

    public AuthAccountService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider)
    {
        _userManager = userManager;
        _jwtProvider = jwtProvider;
    }
    public Task<(Result Result, string UserId)> ForgotPassword(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<(Result Result, LoginResponse? LoginResponse)> Login(string username, string password)
    {
        var user = await _userManager.FindByEmailAsync(username)
            ?? await _userManager.FindByNameAsync(username);

        Guard.Against.NotFound(username, CommonMessage.WRONG_USERNAME_PASSWORD);

        var result = await _userManager.CheckPasswordAsync(user, password);
        if (!result)
        {
            return (Result.Failure(new List<string> { CommonMessage.WRONG_USERNAME_PASSWORD }), null);
        }

        var token = await _jwtProvider.GenerateJwtAsync(user.Id);

        var loginResponse = new LoginResponse
        {
            Token = token,
            TokenType = "Bearer",
            TokenLimit = 10
        };

        return !string.IsNullOrEmpty(token)
            ? (Result.Success(), loginResponse)
            : (Result.Failure(new List<string> { CommonMessage.WRONG_USERNAME_PASSWORD }), null);
    }

    public Task<(Result Result, string UserId)> ResetPassword(string email)
    {
        throw new NotImplementedException();
    }
}
