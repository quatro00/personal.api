using System.Security.Claims;

namespace personal.api.Helpers
{
    public interface IClaimsPrincipalHelper
    {
        Claim? GetUserId(ClaimsPrincipal principal);
    }
    public class ClaimsPrincipalHelper : IClaimsPrincipalHelper
    {
        public Claim? GetUserId(ClaimsPrincipal principal)
        {
            return principal.Claims.FirstOrDefault(item => item.Type.Equals("Id", StringComparison.Ordinal));
        }
    }
    public static class ExtensionMethod
    {
        public static IClaimsPrincipalHelper claimsPrincipalHelper { get; set; }
        public static string GetId(this ClaimsPrincipal User)
        {
            return User.FindFirst("Id").Value;
        }
        public static string GetOrganizacionId(this ClaimsPrincipal User)
        {
            return User.FindFirst("OrganizacionId").Value;
        }
    }
}
