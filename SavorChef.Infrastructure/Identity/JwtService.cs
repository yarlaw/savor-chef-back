using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SavorChef.Application.Common.Interfaces;
using SavorChef.Application.Common.Models;

namespace SavorChef.Infrastructure.Identity;

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly TimeProvider _timeProvider;
    private readonly ITokenRepository _tokenRepository;

    public JwtService(
        JwtSettings jwtSettings,
        UserManager<ApplicationUser> userManager,
        TimeProvider timeProvider,
        ITokenRepository tokenRepository)
    {
        _jwtSettings = jwtSettings;
        _timeProvider = timeProvider;
        _tokenRepository = tokenRepository;
    }

    public async Task<JwtTokenResult> GenerateTokenPairAsync(string userId, string email, string username)
    {
        var now = _timeProvider.GetUtcNow().DateTime;

        var accessTokenExpires = now.AddMinutes(15);
        var refreshTokenExpires = now.AddDays(_jwtSettings.ExpirationDays ?? 7);

        var accessToken = GenerateAccessToken(userId, email, username, accessTokenExpires);
        var refreshToken = GenerateRefreshToken();

        // Store refresh token in repository
        await _tokenRepository.StoreRefreshTokenAsync(new RefreshToken
        {
            Token = refreshToken,
            Email = email,
            UserName = username,
            UserId = userId,
            CreatedAt = now,
            ExpiresAt = refreshTokenExpires,
            IsRevoked = false
        });

        return new JwtTokenResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = accessTokenExpires,
            RefreshTokenExpiresAt = refreshTokenExpires
        };
    }

    public async Task<JwtTokenResult> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _tokenRepository.GetRefreshTokenAsync(refreshToken);

        if (storedToken == null || !storedToken.IsActive)
        {
            throw new SecurityTokenException("Invalid refresh token");
        }

        var userId = storedToken.UserId;
        var email = storedToken.Email!;
        var username = storedToken.UserName!;

        await _tokenRepository.RevokeRefreshTokenAsync(refreshToken);

        return await GenerateTokenPairAsync(userId, email, username);
    }

    public async Task RevokeAllUserTokensAsync(string userId)
    {
        await _tokenRepository.RevokeAllUserTokensAsync(userId);
    }

    public async Task RevokeTokenAsync(string refreshToken)
    {
        await _tokenRepository.RevokeRefreshTokenAsync(refreshToken);
    }

    private string GenerateAccessToken(string userId, string email, string username, DateTime expires)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key))
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
        catch (Exception)
        {
            throw new SecurityTokenException("Invalid token");
        }
    }
}