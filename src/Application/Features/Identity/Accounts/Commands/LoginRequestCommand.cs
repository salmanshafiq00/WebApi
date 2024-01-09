using Application.Common.Interfaces;

namespace Application.Features.Identity.Accounts.Commands;

public record LoginRequestCommand(string UserName, string Password) : IRequest<LoginResponse>;

internal sealed class LoginRequestCommandHandler : IRequestHandler<LoginRequestCommand, LoginResponse>
{
    private readonly IAuthAccountService _authAccountService;

    public LoginRequestCommandHandler(IAuthAccountService authAccountService)
    {
        _authAccountService = authAccountService;
    }
    public async Task<LoginResponse> Handle(LoginRequestCommand request, CancellationToken cancellationToken)
    {
        var result = await _authAccountService.Login(request.UserName, request.Password);
        return result.LoginResponse;
    }
}
