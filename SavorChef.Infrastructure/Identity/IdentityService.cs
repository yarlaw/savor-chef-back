using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SavorChef.Application.Common.Interfaces;
using SavorChef.Application.Common.Models;
using SavorChef.Application.Models;

namespace SavorChef.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthorizationService _authorizationService;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IJwtService _jwtService;
    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IAuthorizationService authorizationService,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _authorizationService = authorizationService;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _jwtService = jwtService;
    }
    
    public async Task<(Result Result, JwtTokenResult? Tokens)> LoginAsync(string identifier,string password)
    {
        var user = await _userManager.FindByNameAsync(identifier) ??
                   await _userManager.FindByEmailAsync(identifier);
        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
        {
            return (Result.Failure(new[] { "Invalid username or password." }), null);
        }

        var tokens = await _jwtService.GenerateTokenPairAsync(user.Id, user.Email!, user.UserName!);
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

        var tokens = await _jwtService.GenerateTokenPairAsync(user.Id, user.Email!, user.UserName!);
        return (Result.Success(), tokens);
    }
    
    public async Task<string?> GetUserEmailAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user?.Email;
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