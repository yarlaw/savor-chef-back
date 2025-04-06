using SavorChef.Application.Common.Interfaces;
using SavorChef.Domain.Entities;
using SavorChef.Domain.Enums;

namespace SavorChef.Application.Recipes.Commands.CreateRecipe;


public class CreateRecipeCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string PreparationInstructions { get; set; } = string.Empty;

    public TimeSpan PreparationTime { get; set; }

    public RecipeDifficulty Difficulty { get; set; }

    public string DishCategory { get; set; } = string.Empty;

    public int CreatorId { get; set; }
}

public class CreateRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, int>
{
    private readonly IApplicationDataContext _context;

    public CreateRecipeCommandHandler(IApplicationDataContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
    {
        var recipe = new Recipe
        {
            Name = request.Name,
            Description = request.Description,
            PreparationInstructions = request.PreparationInstructions,
            PreparationTime = request.PreparationTime,
            Difficulty = request.Difficulty,
            DishCategory = request.DishCategory,
            CreatorId = request.CreatorId
        };

        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync(cancellationToken);

        return recipe.Id;
    }
}