using SavorChef.Api.Infrastructure;
using SavorChef.Infrastructure.Identity;

namespace SavorChef.Api.Endpoints;

public class Users: EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapIdentityApi<ApplicationUser>();
    }
}