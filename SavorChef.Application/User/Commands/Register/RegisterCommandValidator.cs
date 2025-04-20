namespace SavorChef.Application.Recipes.Commands.CreateRecipe;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(v => v.Identifier)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(v => v.Password)
            .NotEmpty();
    }
}
