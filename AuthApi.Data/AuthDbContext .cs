using AuthApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Data
{
	public class AuthDbContext : IdentityDbContext<ApplicationUser>
	{
		public AuthDbContext(DbContextOptions<AuthDbContext> options): base(options)
		{
		}

		public DbSet<ApplicationUser> ApplicationUsers { get; set; }
		public DbSet<RefreshToken> RefreshTokens { get; set; }
	}
}

