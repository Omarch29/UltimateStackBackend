using System.Security.Claims;

namespace REPM.Infrastructure.Security;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets authenticated user's REPM user Guid value.
    /// </summary>
    /// <returns>REPM user Guid value.</returns>
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var claimValueString = principal.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
        return !string.IsNullOrWhiteSpace(claimValueString) ? Guid.Parse(claimValueString) : Guid.Empty;
    }
}