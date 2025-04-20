using Microsoft.AspNetCore.Identity;
using SavorChef.Application.Models;

namespace SavorChef.Infrastructure.Identity;

public static class SIdentityResultExtensions
{
    public static Result ToApplicationResult(this IdentityResult result)
    {
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(result.Errors.Select(e => e.Description));
    }
}