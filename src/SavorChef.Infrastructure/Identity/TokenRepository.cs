using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using SavorChef.Application.Common.Interfaces;
using SavorChef.Application.Common.Models;

namespace SavorChef.Infrastructure.Identity;

public class TokenRepository: ITokenRepository
{
    private readonly IDistributedCache _cache;
    private readonly TimeProvider _timeProvider;
    
    private const string TokenKeyPrefix = "refresh_token:";
    
    // Prefix for user tokens in Redis (used to track all tokens for a user)
    private const string UserTokensKeyPrefix = "user_tokens:";

    public TokenRepository(IDistributedCache cache, TimeProvider timeProvider)
    {
        _cache = cache;
        _timeProvider = timeProvider;
    }

    public async Task StoreRefreshTokenAsync(RefreshToken refreshToken)
    {
        // Store the token
        var tokenKey = TokenKeyPrefix + refreshToken.Token;
        var tokenJson = JsonSerializer.Serialize(refreshToken);
        
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = refreshToken.ExpiresAt
        };
        
        await _cache.SetStringAsync(tokenKey, tokenJson, options);
        
        // Add the token to user's token list
        var userTokensKey = UserTokensKeyPrefix + refreshToken.UserId;
        var userTokensJson = await _cache.GetStringAsync(userTokensKey);
        var userTokens = string.IsNullOrEmpty(userTokensJson) 
            ? new List<string>() 
            : JsonSerializer.Deserialize<List<string>>(userTokensJson) ?? new List<string>();
        
        userTokens.Add(refreshToken.Token);
        
        await _cache.SetStringAsync(
            userTokensKey, 
            JsonSerializer.Serialize(userTokens),
            new DistributedCacheEntryOptions
            {
                // Set a longer expiration for the user tokens collection
                AbsoluteExpiration = _timeProvider.GetUtcNow().DateTime.AddDays(30)
            });
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        var tokenKey = TokenKeyPrefix + token;
        var tokenJson = await _cache.GetStringAsync(tokenKey);
        
        if (string.IsNullOrEmpty(tokenJson))
            return null;
            
        return JsonSerializer.Deserialize<RefreshToken>(tokenJson);
    }

    public async Task RevokeRefreshTokenAsync(string token)
    {
        var refreshToken = await GetRefreshTokenAsync(token);
        
        if (refreshToken == null)
            return;
            
        refreshToken.IsRevoked = true;
        
        var tokenKey = TokenKeyPrefix + token;
        var tokenJson = JsonSerializer.Serialize(refreshToken);
        
        // Keep the token in the cache but mark it as revoked
        // This helps prevent replay attacks with the same token
        await _cache.SetStringAsync(
            tokenKey, 
            tokenJson,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = refreshToken.ExpiresAt
            });
    }

    public async Task RevokeAllUserTokensAsync(string userId)
    {
        var userTokensKey = UserTokensKeyPrefix + userId;
        var userTokensJson = await _cache.GetStringAsync(userTokensKey);
        
        if (string.IsNullOrEmpty(userTokensJson))
            return;
            
        var userTokens = JsonSerializer.Deserialize<List<string>>(userTokensJson) ?? new List<string>();
        
        // Revoke each token
        foreach (var token in userTokens)
        {
            await RevokeRefreshTokenAsync(token);
        }
        
        // Clear the user's token list
        await _cache.RemoveAsync(userTokensKey);
    }

    public async Task CleanupExpiredTokensAsync()
    {
        await Task.CompletedTask;
    }
}