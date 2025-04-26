using SavorChef.Application.Common.Models;

namespace SavorChef.Application.Common.Interfaces;

public interface IJwtService
{
    Task<JwtTokenResult> GenerateTokenPairAsync(string userId, string email, string username);
    
    Task<JwtTokenResult> RefreshTokenAsync(string refreshToken);
    
    Task RevokeAllUserTokensAsync(string userId);

    Task RevokeTokenAsync(string refreshToken);
}