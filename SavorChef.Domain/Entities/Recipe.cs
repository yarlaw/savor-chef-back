using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SavorChef.Domain.Common;
using SavorChef.Domain.Enums;

namespace SavorChef.Domain.Entities;

public class Recipe : BaseAuditableEntity
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PreparationInstructions { get; set; } = string.Empty;
    public TimeSpan PreparationTime { get; set; }
    public RecipeDifficulty Difficulty { get; set; }
    public string DishCategory { get; set; } = string.Empty;

    [Required]
    public int CreatorId { get; set; }
}

