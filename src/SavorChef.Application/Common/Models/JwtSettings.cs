namespace SavorChef.Application.Common.Models;

public class JwtSettings
{
    public string Key { get; set; } = default!;
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    
    public double? ExpirationDays { get; set; }
}