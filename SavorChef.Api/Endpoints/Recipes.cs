using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SavorChef.Api.Infrastructure;
using SavorChef.Application.Common.Interfaces;
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

    private static async Task<Results<Created<int>, UnauthorizedHttpResult>> CreateRecipe(ISender sender, CreateRecipeCommand command, IUser currentUser)
    {
        
        var userId = currentUser.Id;
        
        
        if (string.IsNullOrEmpty(userId))
        {
            return TypedResults.Unauthorized();
        }

        var createCommandWithCreator = new CreateRecipeCommand
        {
            Name = command.Name,
            Description = command.Description,
            PreparationInstructions = command.PreparationInstructions,
            PreparationTimeValue = command.PreparationTimeValue,
            PreparationTimeUnit = command.PreparationTimeUnit,
            Difficulty = command.Difficulty,
            DishCategory = command.DishCategory,
            CreatorId = userId,
        };
        
        var id = await sender.Send(createCommandWithCreator);
        
        Console.WriteLine(id);

        return TypedResults.Created($"/{nameof(Recipe)}/{id}", id);
    }

}