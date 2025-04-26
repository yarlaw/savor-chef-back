using SavorChef.Application.User.Commands.RefreshToken;

namespace SavorChef.Application.User.Commands.RefreshToken;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(v => v.RefreshToken)
            .NotEmpty();
    }
}
