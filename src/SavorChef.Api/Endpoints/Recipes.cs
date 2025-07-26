using Microsoft.AspNetCore.Http.HttpResults;
using SavorChef.Api.Infrastructure;
using SavorChef.Application.Common.Interfaces;
using SavorChef.Application.Recipes.Commands.CreateRecipe;
using SavorChef.Application.Recipes.Queries.GetRecipesWithPagination;

namespace SavorChef.Api.Endpoints;

public class Recipes : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {

        var recipes = app.MapGroup("/recipes").RequireAuthorization();

        recipes.MapGet("/", GetRecipes)
                .WithDescription("Get a paginated list of recipes with optional filtering")
                .WithOpenApi(operation =>
                {
                    operation.Parameters.First(p => p.Name == "pageNumber").Description = "Page number (1-based, default: 1)";
                    operation.Parameters.First(p => p.Name == "pageSize").Description = "Number of items per page (default: 10, max: 50)";
                    operation.Parameters.First(p => p.Name == "searchTerm").Description = "Optional term to search in recipe name and description";
                    operation.Parameters.First(p => p.Name == "difficulty").Description = "Optional filter by difficulty (0=Easy, 1=Medium, 2=Hard)";
                    operation.Parameters.First(p => p.Name == "creatorId").Description = "Optional filter by creator ID";
                    return operation;
                });

        recipes.MapPost("/", CreateRecipe)
                .WithDescription("Create a new recipe");
    }

    private async Task<Ok<PaginatedRecipesVm>> GetRecipes(
        ISender sender,
        int? pageNumber = 1,
        int? pageSize = 10,
        string? searchTerm = null,
        int? difficulty = null,
        string? creatorId = null)
    {
        var query = new GetRecipesWithPaginationQuery
        {
            PageNumber = pageNumber ?? 1,
            PageSize = pageSize ?? 10,
            SearchTerm = searchTerm,
            Difficulty = difficulty.HasValue ? (Domain.Enums.RecipeDifficulty)difficulty.Value : null,
            CreatorId = creatorId
        };

        var paginatedList = await sender.Send(query);
        var vm = PaginatedRecipesVm.FromPaginatedList(paginatedList);

        return TypedResults.Ok(vm);
    }

    private async Task<Created<Guid>> CreateRecipe(ISender sender, CreateRecipeCommand command, IUser user)
    {
        command.CreatorId = user.Id!;

        var id = await sender.Send(command);

        return TypedResults.Created($"/{nameof(Recipes)}/{id}", id);
    }
}
