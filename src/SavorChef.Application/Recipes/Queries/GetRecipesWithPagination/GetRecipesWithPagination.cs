using SavorChef.Application.Common.Interfaces;
using SavorChef.Application.Models;
using Microsoft.Extensions.DependencyInjection.Recipes.Queries.GetRecipes;
using SavorChef.Domain.Enums;

namespace SavorChef.Application.Recipes.Queries.GetRecipesWithPagination;

public class GetRecipesWithPaginationQuery : IRequest<PaginatedList<RecipeDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public RecipeDifficulty? Difficulty { get; set; }
    public string? CreatorId { get; set; }
}

public class GetRecipesWithPaginationQueryHandler : IRequestHandler<GetRecipesWithPaginationQuery, PaginatedList<RecipeDto>>
{
    private readonly IApplicationDataContext _context;
    private readonly IMapper _mapper;

    public GetRecipesWithPaginationQueryHandler(IApplicationDataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<RecipeDto>> Handle(GetRecipesWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Recipes.AsQueryable();

        // Apply filtering
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(r =>
                r.Name.ToLower().Contains(searchTerm) ||
                r.Description.ToLower().Contains(searchTerm));
        }

        if (request.Difficulty.HasValue)
        {
            query = query.Where(r => r.Difficulty == request.Difficulty.Value);
        }

        if (!string.IsNullOrEmpty(request.CreatorId))
        {
            query = query.Where(r => r.CreatorId == request.CreatorId);
        }

        // Apply ordering (assuming recipes have a Created property, adjust as needed)
        query = query.OrderByDescending(r => r.Id);

        // Project to DTO and paginate
        return await query
            .ProjectTo<RecipeDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
