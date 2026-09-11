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

		#region Register
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

			// Assign default role
			if (!await _roleManager.RoleExistsAsync("User"))
				await _roleManager.CreateAsync(new IdentityRole { Name = "User" });
			
			IdentityResult roleResult = await _userManager.AddToRoleAsync(user, "User");
			if (!roleResult.Succeeded)
			{
				string errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
				return BadRequest(ApiResponse<RegisterDto>.FailResponse(errors, "Role assignment failed"));
			}

			RegisterResponseDto response = _mapper.Map<RegisterResponseDto>(user);
			return Ok(ApiResponse<RegisterResponseDto>.SuccessResponse(response, "User registered successfully", 201));
		}
		#endregion

		#region Login
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
			if (user == null)
				return Unauthorized(ApiResponse<LoginDto>.FailResponse(
					error: "Invalid credentials",
					message: "Login failed",
					statusCode: StatusCodes.Status401Unauthorized));

			if (!await _userManager.CheckPasswordAsync(user, dto.Password))
				return Unauthorized(ApiResponse<LoginDto>.FailResponse(
					error: "Invalid credentials",
					message: "Login failed",
					statusCode: StatusCodes.Status401Unauthorized));

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

			LoginResponseDto loginResponse = new()
			{
				AccessToken = token,
				RefreshToken = refreshToken,
				Roles = roles
			};
			return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(loginResponse, "Login successful", StatusCodes.Status200OK));
		}
		#endregion

		#region Refresh
		[HttpPost(nameof(Refresh))]
		public async Task<IActionResult> Refresh(string refreshToken)
		{
			RefreshToken savedToken = await _authService.FindSingleAsync(t => t.Token == refreshToken);


			if (savedToken == null || savedToken.IsRevoked || savedToken.Expires < DateTime.UtcNow)
				return Unauthorized(ApiResponse<string>.FailResponse("Invalid or expired refresh token"));

			ApplicationUser? user = await _userManager.FindByIdAsync(savedToken.UserId);
			if (user == null) return Unauthorized(ApiResponse<string>.FailResponse("User not found"));

			IList<string> roles = await _userManager.GetRolesAsync(user);
			string newAccessToken = _authService.CreateToken(user, roles);
			string newRefreshToken = _authService.GenerateRefreshToken();

			await _authService.RevokeRefreshToken(savedToken.Id);
	
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
		#endregion

	}
}
