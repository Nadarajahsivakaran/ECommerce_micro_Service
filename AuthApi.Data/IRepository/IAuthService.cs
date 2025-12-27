
using AuthApi.Models;
using ECommerce.Data;
using ProductApi.Models;

namespace AuthApi.Data.IRepository
{
    public interface IAuthService : IGenericRepository<RefreshToken>
	{
        string CreateToken(ApplicationUser user, IList<string> roles);

        string GenerateRefreshToken();

        Task  RevokeRefreshToken(Guid id);

       
	}
}
