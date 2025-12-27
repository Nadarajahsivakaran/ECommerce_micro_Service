using AuthApi.Data.IRepository;
using AuthApi.Models;
using AuthApi.Models.DTO;
using AutoMapper;
using ECommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IAuthService authService, IMapper mapper) : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager = userManager;
		private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IAuthService _authService = authService;
		private readonly IMapper _mapper = mapper;


        [HttpPost(nameof(Register))]
		public async Task<IActionResult> Register(RegisterDto dto)
		{
			if (!ModelState.IsValid)
			{
				string errors = string.Join("; ", ModelState.Values
												 .SelectMany(v => v.Errors)
												 .Select(e => e.ErrorMessage));

				return BadRequest(ApiResponse<RegisterDto>.FailResponse(errors, "Validation failed"));
			}

			ApplicationUser user = _mapper.Map<ApplicationUser>(dto);

			IdentityResult? result = await _userManager.CreateAsync(user, dto.Password);
			if (!result.Succeeded)
			{
				string errors = string.Join("; ", result.Errors.Select(e => e.Description));
				return BadRequest(ApiResponse<RegisterDto>.FailResponse(errors, "Registration failed"));
			}

			// 4️⃣ Ensure role exists and assign it
			string roleName = dto.Role.ToString(); // enum -> string
			if (!await _roleManager.RoleExistsAsync(roleName))
				await _roleManager.CreateAsync(new IdentityRole { Name = roleName });

			await _userManager.AddToRoleAsync(user, roleName);

			RegisterResponseDto response = _mapper.Map<RegisterResponseDto>(user);
			return Ok(ApiResponse<RegisterResponseDto>.SuccessResponse(response, "User registered successfully",201));
		}

		[HttpPost(nameof(Login))]
		public async Task<IActionResult> Login(LoginDto dto)
		{
			if (!ModelState.IsValid)
			{
				string errors = string.Join("; ", ModelState.Values
												 .SelectMany(v => v.Errors)
												 .Select(e => e.ErrorMessage));

				return BadRequest(ApiResponse<RegisterDto>.FailResponse(errors, "Validation failed"));
			}

			ApplicationUser? user = await _userManager.FindByEmailAsync(dto.Email);
			if (user == null) return Unauthorized(ApiResponse<LoginDto>.FailResponse("Invalid credentials", "Login failed", 401));

			if (!await _userManager.CheckPasswordAsync(user, dto.Password))
				return Unauthorized(ApiResponse<LoginDto>.FailResponse("Invalid credentials", "Login failed", 401));

			IList<string> roles = await _userManager.GetRolesAsync(user);
			
			// Generate Access Token
			string token = _authService.CreateToken(user, roles);

			// Generate Refresh Token
			string refreshToken = _authService.GenerateRefreshToken();
			
			await _authService.AddAsync(new RefreshToken
			{
				Token = refreshToken,
				UserId = user.Id,
				Expires = DateTime.UtcNow.AddDays(7)
			});


			return Ok(ApiResponse<object>.SuccessResponse(
				new{
						AccessToken = token,
						RefreshToken = refreshToken
					}, "Login successful"));
		}

		[HttpPost(nameof(Refresh))]
		public async Task<IActionResult> Refresh(string refreshToken)
		{
			RefreshToken savedToken = await _authService.FindSingleAsync(t=>t.Token == refreshToken);


			if (savedToken == null || savedToken.IsRevoked || savedToken.Expires < DateTime.UtcNow)
				return Unauthorized(ApiResponse<string>.FailResponse("Invalid or expired refresh token"));

			ApplicationUser? user = await _userManager.FindByIdAsync(savedToken.UserId);
			if (user == null) return Unauthorized(ApiResponse<string>.FailResponse("User not found"));

			IList<string> roles = await _userManager.GetRolesAsync(user);
			string newAccessToken = _authService.CreateToken(user, roles);
			string newRefreshToken = _authService.GenerateRefreshToken();

			// Revoke old token
			await _authService.RevokeRefreshToken(savedToken.Id);

			// Save new refresh token
			await _authService.AddAsync(new RefreshToken
			{
				Token = newRefreshToken,
				UserId = user.Id,
				Expires = DateTime.UtcNow.AddDays(7)
			});

			return Ok(ApiResponse<object>.SuccessResponse(new
			{
				AccessToken = newAccessToken,
				RefreshToken = newRefreshToken,
				savedToken
			}, "Token refreshed successfully"));
		}

	}
}
