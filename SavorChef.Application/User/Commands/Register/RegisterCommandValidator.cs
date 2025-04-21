namespace SavorChef.Application.User.Commands.Register;

public class LoginCommandValidator : AbstractValidator<RegisterCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(v => v.Username).
            NotEmpty()
            .MaximumLength(256);
        
        RuleFor(v => v.Email)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(v => v.Password)
            .NotEmpty();
    }
}
