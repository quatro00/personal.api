using Microsoft.AspNetCore.Identity;
using personal.api.Models;

namespace personal.api.Repositories.Interface
{
    public interface ITokenRepository
    {
        string CreateJwtToken(ApplicationUser user, List<string> roles);
        string CreateRestoreToken(ApplicationUser user);
    }
}
