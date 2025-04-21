using SavorChef.Application.Common.Interfaces;
using SavorChef.Application.Common.Models;

namespace SavorChef.Application.User.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<JwtTokenResult>
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, JwtTokenResult>
{
    private readonly IJwtService _jwtService;

    public RefreshTokenCommandHandler(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    public async Task<JwtTokenResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            return await _jwtService.RefreshTokenAsync(request.RefreshToken);
        }
        catch (Exception ex)
        {
            throw new UnauthorizedAccessException("Invalid refresh token", ex);
        }
    }
}