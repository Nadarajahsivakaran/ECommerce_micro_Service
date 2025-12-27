using AuthApi.Data.IRepository;
using AuthApi.Models;
using ECommerce.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthApi.Data.Repository
{
	public class AuthService : GenericRepository<RefreshToken>, IAuthService
	{
		private readonly IConfiguration _config;
		private readonly AuthDbContext _context;

		public AuthService(AuthDbContext context, IConfiguration config) : base(context)
		{
			_context = context;
			_config = config;
		}

		public string CreateToken(ApplicationUser user, IList<string> roles)
		{
			var claims = new List<Claim>
			{
				new(JwtRegisteredClaimNames.Sub, user.Id),
				new(JwtRegisteredClaimNames.Email, user.Email),
				new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};

			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _config["Jwt:Issuer"],
				audience: _config["Jwt:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpireMinutes"])),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public string GenerateRefreshToken()
		{
			byte[] randomBytes = new byte[32];
			RNGCryptoServiceProvider rng = new();
			rng.GetBytes(randomBytes);
			return Convert.ToBase64String(randomBytes);
		}

		public async Task RevokeRefreshToken(Guid id)
		{
			RefreshToken? token = await GetByIdAsync(id);

            if(token == null)
				throw new KeyNotFoundException("Refresh token not found");

			token.IsRevoked = true;
			await Update(token);
		}

       
    }
}
