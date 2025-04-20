using SavorChef.Application.Common.Models;
using SavorChef.Application.Models;

namespace SavorChef.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserEmailAsync(string userId);
    Task<bool> AuthorizeAsync(string userId, string policyName);
    Task<(Result Result, JwtTokenResult? Tokens)> LoginAsync(string identifier, string password);
    Task<(Result Result, JwtTokenResult? Tokens)> SignUpAsync(string userName, string email, string password);
    Task<Result> DeleteUserAsync(string userId);
}