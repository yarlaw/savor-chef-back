namespace SavorChef.Application.User.Commands.Login;
public class RegisterCommandValidator : AbstractValidator<LoginCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(v => v.Identifier)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(v => v.Password)
            .NotEmpty()
            .MinimumLength(6);
    }
}
