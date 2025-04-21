using Microsoft.AspNetCore.Http.HttpResults;
using SavorChef.Api.Infrastructure;
using SavorChef.Application.Common.Interfaces;
using SavorChef.Application.User.Commands.Login;
using SavorChef.Application.User.Commands.RefreshToken;
using SavorChef.Application.User.Commands.Register;

namespace SavorChef.Api.Endpoints;

public class Users: EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup("/auth")
            .WithTags("Auth")
            .AllowAnonymous();

        group.MapPost("/login", Login);
        group.MapPost("/register", Register);
        group.MapPost("/refresh", RefreshToken);
        group.MapPost("/logout", Logout);
    }
    
    private async Task<Results<Ok<AccessTokenResponse>, BadRequest<string>>> Login(
        ISender sender,
        LoginCommand command,
        HttpContext httpContext)
    {
        try
        {
            var result = await sender.Send(command);
            
            SetRefreshTokenCookie(httpContext, result.RefreshToken, result.RefreshTokenExpiresAt);
            
            return TypedResults.Ok(new AccessTokenResponse{AccessToken = result.AccessToken, ExpiresAt = result.AccessTokenExpiresAt});
        }
        catch (UnauthorizedAccessException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    private async Task<Results<Ok<AccessTokenResponse>, BadRequest<string>>> Register(
        ISender sender,
        RegisterCommand command,
        HttpContext httpContext)
    {
        try
        {
            var result = await sender.Send(command);
            
            SetRefreshTokenCookie(httpContext, result.RefreshToken, result.RefreshTokenExpiresAt);
            
            return TypedResults.Ok(new AccessTokenResponse{AccessToken = result.AccessToken, ExpiresAt = result.AccessTokenExpiresAt});
        }
        catch (ApplicationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
    
    
    private void SetRefreshTokenCookie(HttpContext httpContext, string refreshToken, DateTime expiresAt)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // For HTTPS
            SameSite = SameSiteMode.Strict,
            Expires = expiresAt,
            Path = "/auth/refresh",
        };
        
        httpContext.Response.Cookies.Append("refresh_token", refreshToken, cookieOptions);
    }
    
    private async Task<Results<Ok<AccessTokenResponse>, BadRequest<string>, UnauthorizedHttpResult>> RefreshToken(
        ISender sender,
        HttpContext httpContext)
    {
        // Get the refresh token from the cookie
        var refreshToken = httpContext.Request.Cookies["refresh_token"];
        
        if (string.IsNullOrEmpty(refreshToken))
        {
            return TypedResults.Unauthorized();
        }
        
        try
        {
            // Create and send the refresh token command
            var command = new RefreshTokenCommand { RefreshToken = refreshToken };
            var result = await sender.Send(command);
            
            // Set the new refresh token in an HTTP only cookie
            SetRefreshTokenCookie(httpContext, result.RefreshToken, result.RefreshTokenExpiresAt);
            
            // Return only the access token in the response body
            return TypedResults.Ok(new AccessTokenResponse
            {
                AccessToken = result.AccessToken,
                ExpiresAt = result.AccessTokenExpiresAt
            });
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
    
    private async Task<Results<NoContent,BadRequest<string>>> Logout(
        ISender sender,
        HttpContext httpContext,
        IJwtService jwtService)
    {
        var refreshToken = httpContext.Request.Cookies["refresh_token"];
        
        if (!string.IsNullOrEmpty(refreshToken))
        {
            await jwtService.RevokeTokenAsync(refreshToken);
        }
        
        httpContext.Response.Cookies.Delete("refresh_token");
        
        return TypedResults.NoContent();
    }
}

public class AccessTokenResponse
{
    public string AccessToken { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
}