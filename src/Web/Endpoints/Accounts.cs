using Application.Features.Identity.Accounts.Commands;
using WebApi.Application.Common.Models;

namespace WebApi.Web.Endpoints;

public class Accounts : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(Login);
    }

    public async Task<LoginResponse> Login(ISender sender, LoginRequestCommand query)
    {
        return await sender.Send(query);
    }
}

