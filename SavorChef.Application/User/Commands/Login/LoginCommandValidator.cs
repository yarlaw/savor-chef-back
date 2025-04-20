namespace SavorChef.Application.Recipes.Commands.CreateRecipe;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(v => v.Username)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(v => v.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(v => v.Password)
            .NotEmpty()
            .MinimumLength(6);
    }
}
