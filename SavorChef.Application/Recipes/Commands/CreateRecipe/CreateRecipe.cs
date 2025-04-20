using System.Text.Json.Serialization;
using SavorChef.Application.Common.Interfaces;
using SavorChef.Domain.Entities;
using SavorChef.Domain.Enums;

namespace SavorChef.Application.Recipes.Commands.CreateRecipe
{
    public class CreateRecipeCommand : IRequest<int>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [JsonIgnore]
        public string CreatorId { get; set; } = string.Empty;
    }

    // Handler for the ping command
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
                CreatorId = request.CreatorId,
                Difficulty = RecipeDifficulty.Easy,
            };
            
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync(cancellationToken);

            return recipe.Id;
        }
    }
}