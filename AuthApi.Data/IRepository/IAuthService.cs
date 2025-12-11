
using AuthApi.Models;

namespace AuthApi.Data.IRepository
{
    public interface IAuthService
    {
        string CreateToken(ApplicationUser user, IList<string> roles);

	}
}
