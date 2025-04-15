using Microsoft.AspNetCore.Http.HttpResults;
using SavorChef.Api.Infrastructure;
using SavorChef.Application.Recipes.Commands.CreateRecipe;
using SavorChef.Domain.Entities;

namespace SavorChef.Api.Endpoints;

public class Recipes: EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(CreateRecipe);
    }

    private static async Task<Created<int>> CreateRecipe(ISender sender, CreateRecipeCommand command)
    {
        var id = await sender.Send(command);

        return TypedResults.Created($"/{nameof(Recipe)}/{id}", id);
    }

}