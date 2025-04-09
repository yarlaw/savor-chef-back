using FluentValidation;

namespace SavorChef.Application.Recipes.Commands.CreateRecipe;

public class CreateRecipeCommandValidator : AbstractValidator<CreateRecipeCommand>
{
    public CreateRecipeCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(x => x.PreparationInstructions)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.PreparationTime)
            .NotEmpty()
            .Must(x => x.TotalMinutes > 0)
            .WithMessage("Preparation time must be greater than 0 minutes");

        RuleFor(x => x.Difficulty)
            .IsInEnum();

        RuleFor(x => x.DishCategory)
            .NotEmpty()
            .MaximumLength(100);
    }
}
