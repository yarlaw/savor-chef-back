using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SavorChef.Application.Common.Interfaces;
using SavorChef.Domain.Entities;
using SavorChef.Domain.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace SavorChef.Application.Recipes.Commands.CreateRecipe;


public class CreateRecipeCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string PreparationInstructions { get; set; } = string.Empty;

    public long PreparationTimeValue { get; set; }
    public string PreparationTimeUnit { get; set; } = string.Empty;

    public RecipeDifficulty Difficulty { get; set; }

    public string DishCategory { get; set; } = string.Empty;
    
    public string CreatorId { get; set; } = string.Empty;
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
        Console.WriteLine("Here");

        TimeSpan preparationTime;
        switch (request.PreparationTimeUnit?.ToLower())
        {
            case "seconds":
                preparationTime = TimeSpan.FromSeconds(request.PreparationTimeValue);
                break;
            case "minutes":
                preparationTime = TimeSpan.FromMinutes(request.PreparationTimeValue);
                break;
            default:
                preparationTime = TimeSpan.FromHours(request.PreparationTimeValue);
                break;
        }
        
        var recipe = new Recipe
        {
            Name = request.Name,
            Description = request.Description,
            PreparationInstructions = request.PreparationInstructions,
            PreparationTime = preparationTime,
            Difficulty = request.Difficulty,
            DishCategory = request.DishCategory,
            CreatorId = request.CreatorId
        };

        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync(cancellationToken);

        return recipe.Id;
    }
}