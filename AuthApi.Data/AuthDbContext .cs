using AuthApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Data
{
    public class AuthDbContext(DbContextOptions<AuthDbContext> options) : IdentityDbContext<ApplicationUser>(options)
	{
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
	}
}

