using SavorChef.Application.Models;
using SavorChef.Application.Recipes.DTOs;

namespace SavorChef.Application.Recipes.Queries.GetRecipesWithPagination;

public class PaginatedRecipesVm
{
    public IReadOnlyCollection<RecipeDto> Recipes { get; init; } = Array.Empty<RecipeDto>();
    public int PageNumber { get; init; }
    public int TotalPages { get; init; }
    public int TotalCount { get; init; }
    public bool HasPreviousPage { get; init; }
    public bool HasNextPage { get; init; }

    public static PaginatedRecipesVm FromPaginatedList(PaginatedList<RecipeDto> list)
    {
        return new PaginatedRecipesVm
        {
            Recipes = list.Items,
            PageNumber = list.PageNumber,
            TotalPages = list.TotalPages,
            TotalCount = list.TotalCount,
            HasPreviousPage = list.HasPreviousPage,
            HasNextPage = list.HasNextPage
        };
    }
}
