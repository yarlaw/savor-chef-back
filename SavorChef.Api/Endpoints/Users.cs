using Microsoft.AspNetCore.Http.HttpResults;
using SavorChef.Api.Infrastructure;
using SavorChef.Application.Common.Models;
using SavorChef.Application.Recipes.Commands.CreateRecipe;
using SavorChef.Infrastructure.Identity;

namespace SavorChef.Api.Endpoints;

public class Users: EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup("/auth")
            .WithTags("Auth")
            .AllowAnonymous();

        group.MapPost("/login", Login);
        group.MapPost("/register", Register);
    }
    
    private async Task<Results<Ok<JwtTokenResult>, BadRequest<string>>> Login(
        ISender sender,
        LoginCommand command)
    {
        try
        {
            var result = await sender.Send(command);
            return TypedResults.Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    private async Task<Results<Ok<JwtTokenResult>, BadRequest<string>>> Register(
        ISender sender,
        RegisterCommand command)
    {
        try
        {
            var result = await sender.Send(command);
            return TypedResults.Ok(result);
        }
        catch (ApplicationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}