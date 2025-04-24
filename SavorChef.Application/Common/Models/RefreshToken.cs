namespace SavorChef.Application.Common.Models;

public class RefreshToken
{
    public string Token { get; set; } = default!;

    public string UserId { get; set; } = default!;

    public string UserName { get; set; } = default!;

    public string Email { get; set; } = default!;

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; }

    public string? PreviousToken { get; set; }

    public bool IsActive => !IsRevoked && ExpiresAt > DateTime.UtcNow;
}