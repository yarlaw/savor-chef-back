using System.ComponentModel.DataAnnotations;
using SavorChef.Domain.Common;

namespace SavorChef.Domain.Entities;


public class User: BaseAuditableEntity
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public ICollection<Recipe> CreatedRecipes { get; set; } = new List<Recipe>();
}