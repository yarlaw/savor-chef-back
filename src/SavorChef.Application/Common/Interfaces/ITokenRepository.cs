using SavorChef.Application.Common.Models;

namespace SavorChef.Application.Common.Interfaces;

public interface ITokenRepository
{
    Task StoreRefreshTokenAsync(RefreshToken refreshToken);
    
    Task<RefreshToken?> GetRefreshTokenAsync(string token);
    
    Task RevokeRefreshTokenAsync(string token);
    
    Task RevokeAllUserTokensAsync(string userId);
    
    Task CleanupExpiredTokensAsync();
}