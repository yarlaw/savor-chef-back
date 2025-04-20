using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SavorChef.Application.Common.Interfaces;
using SavorChef.Application.Common.Models;
using SavorChef.Application.Models;

namespace SavorChef.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthorizationService _authorizationService;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly JwtSettings _jwtSettings;
    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IAuthorizationService authorizationService,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        JwtSettings jwtSettings)
    {
        _userManager = userManager;
        _authorizationService = authorizationService;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _jwtSettings = jwtSettings;
    }
    
    public async Task<(Result Result, JwtTokenResult? Tokens)> LoginAsync(string identifier,string password)
    {
        var user = await _userManager.FindByNameAsync(identifier) ??
                   await _userManager.FindByEmailAsync(identifier);
        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
        {
            return (Result.Failure(new[] { "Invalid username or password." }), null);
        }

        var tokens = GenerateTokenAsync(user);
        return (Result.Success(), tokens);
    }
    
    public async Task<(Result Result, JwtTokenResult? Tokens)> SignUpAsync(string userName, string email, string password)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = email,
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return (result.ToApplicationResult(), null);
        }

        var tokens = GenerateTokenAsync(user);
        return (Result.Success(), tokens);
    }
    
    
    public async Task<string?> GetUserEmailAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user?.Email;
    }
    
    private JwtTokenResult GenerateTokenAsync(ApplicationUser user)
    {
        var now = DateTime.UtcNow;

        var accessExpires = now.AddMinutes(15);
        var refreshExpires = now.AddDays(_jwtSettings.ExpirationDays ?? 7);

        var accessToken = GenerateJwtToken(user, accessExpires);
        var refreshToken = GenerateJwtToken(user, refreshExpires);


        return new JwtTokenResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = accessExpires,
            RefreshTokenExpiresAt = refreshExpires
        };
    }
    
    private string GenerateJwtToken(ApplicationUser user, DateTime expires)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
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

    public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string email,  string password)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = email,
        };

        var result = await _userManager.CreateAsync(user, password);

        return (result.ToApplicationResult(), user.Id);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null ? await DeleteUserAsync(user) : Result.Success();
    }

    public async Task<Result> DeleteUserAsync(ApplicationUser user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }
}