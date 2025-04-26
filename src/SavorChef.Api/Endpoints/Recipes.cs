using Microsoft.AspNetCore.Http.HttpResults;
using SavorChef.Api.Infrastructure;
using SavorChef.Api.Services;
using SavorChef.Application.Common.Interfaces;
using SavorChef.Application.Recipes.Commands.CreateRecipe;

namespace SavorChef.Api.Endpoints;

public class Recipes: EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(CreateRecipe);
    }

    private async Task<Created<Guid>> CreateRecipe(ISender sender, CreateRecipeCommand command, IUser user)
    {
        command.CreatorId = user.Id!;
        
        var id = await sender.Send(command);

        return TypedResults.Created($"/{nameof(Recipes)}/{id}", id);
    }
}