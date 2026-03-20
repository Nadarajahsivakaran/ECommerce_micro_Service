using AuthApi.Models;
using Microsoft.AspNetCore.Identity;

public static class DbInitializer
{
	public static async Task SeedAsync(IServiceProvider serviceProvider)
	{
		using var scope = serviceProvider.CreateScope();
		var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
		var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

		// Create Admin Role
		if (!await roleManager.RoleExistsAsync("Admin"))
		{
			await roleManager.CreateAsync(new IdentityRole("Admin"));
		}

		// Create Admin User
		var adminEmail = "admin@gmail.com";
		var adminPassword = "Admin@123";

		var adminUser = await userManager.FindByEmailAsync(adminEmail);
		if (adminUser == null)
		{
			adminUser = new ApplicationUser
			{
				UserName = adminEmail,
				Email = adminEmail,
				EmailConfirmed = true
			};

			await userManager.CreateAsync(adminUser, adminPassword);
			await userManager.AddToRoleAsync(adminUser, "Admin");
		}
	}
}
