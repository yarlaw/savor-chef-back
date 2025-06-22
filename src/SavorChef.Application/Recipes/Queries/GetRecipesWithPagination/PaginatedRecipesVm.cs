using SavorChef.Application.Models;
using Microsoft.Extensions.DependencyInjection.Recipes.Queries.GetRecipes;
using System.Linq;

namespace SavorChef.Application.Recipes.Queries.GetRecipesWithPagination;

public class PaginatedRecipesVm
{
    public IReadOnlyCollection<RecipeDto> Items { get; init; } = Array.Empty<RecipeDto>();
    public int PageNumber { get; init; }
    public int TotalPages { get; init; }
    public int TotalCount { get; init; }
    public bool HasPreviousPage { get; init; }
    public bool HasNextPage { get; init; }

    public static PaginatedRecipesVm FromPaginatedList(PaginatedList<RecipeDto> list)
    {
        return new PaginatedRecipesVm
        {
            Items = list.Items,
            PageNumber = list.PageNumber,
            TotalPages = list.TotalPages,
            TotalCount = list.TotalCount,
            HasPreviousPage = list.HasPreviousPage,
            HasNextPage = list.HasNextPage
        };
    }

    /// <summary>
    /// Converts this paginated result to the legacy RecipesVm format for backward compatibility
    /// </summary>
    public RecipesVm ToLegacyVm()
    {
        return new RecipesVm
        {
            Recipes = Items.ToList()
        };
    }
}
