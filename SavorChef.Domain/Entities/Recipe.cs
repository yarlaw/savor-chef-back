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
    public RecipeDifficulty Difficulty { get; set; } = RecipeDifficulty.Easy;
    [Required]
    public string CreatorId { get; set; } = string.Empty;
} 

