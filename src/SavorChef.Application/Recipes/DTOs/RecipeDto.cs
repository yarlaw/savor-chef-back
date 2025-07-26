using SavorChef.Domain.Enums;

namespace SavorChef.Application.Recipes.DTOs;

public class RecipeDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string CreatorId { get; set; } = string.Empty;

    public RecipeDifficulty Difficulty { get; set; }
}