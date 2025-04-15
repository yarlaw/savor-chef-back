using System.Reflection;
using SavorChef.Application.Common.Exceptions;
using SavorChef.Application.Common.Interfaces;
using SavorChef.Application.Common.Security;

namespace SavorChef.Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public AuthorizationBehaviour(
        IUser user,
        IIdentityService identityService)
    {
        _user = user;
        _identityService = identityService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

        var attributes = authorizeAttributes.ToList();
        if (attributes.Count == 0) return await next();

        if (_user.Id == null)
        {
            throw new UnauthorizedAccessException();
        }

        // Role-based authorization
        // var authorizeAttributesWithRoles = attributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));
        // var attributesWithRoles = authorizeAttributesWithRoles.ToList();
        //
        // if (attributesWithRoles.Count != 0)
        // {
        //     var authorized = false;
        //
        //     foreach (var roles in attributesWithRoles.Select(a => a.Roles.Split(',')))
        //     {
        //         foreach (var role in roles)
        //         {
        //             var isInRole = await _identityService.IsInRoleAsync(_user.Id, role.Trim());
        //             if (isInRole)
        //             {
        //                 authorized = true;
        //                 break;
        //             }
        //         }
        //     }
        //
        //     // Must be a member of at least one role in roles
        //     if (!authorized)
        //     {
        //         throw new ForbiddenAccessException();
        //     }
        // }

        // User is authorized / authorization not required
        return await next();
    }
}