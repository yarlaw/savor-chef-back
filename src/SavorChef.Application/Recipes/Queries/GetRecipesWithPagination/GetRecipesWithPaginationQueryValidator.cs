using SavorChef.Application.Recipes.Queries.GetRecipesWithPagination;

namespace SavorChef.Application.Recipes.Queries.GetRecipesWithPagination;

public class GetRecipesWithPaginationQueryValidator : AbstractValidator<GetRecipesWithPaginationQuery>
{
    public GetRecipesWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("PageNumber must be at least 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage("PageSize must be at least 1.")
            .LessThanOrEqualTo(50)
            .WithMessage("PageSize must not exceed 50.");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.SearchTerm));

        RuleFor(x => x.Difficulty)
            .IsInEnum()
            .When(x => x.Difficulty.HasValue);
    }
}
