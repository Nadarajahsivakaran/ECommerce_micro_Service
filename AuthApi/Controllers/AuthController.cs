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

			// Assign default role
			if (!await _roleManager.RoleExistsAsync("User"))
				await _roleManager.CreateAsync(new IdentityRole { Name = "User" });
			await _userManager.AddToRoleAsync(user, "User");

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
			string token = _authService.CreateToken(user, roles);
			return Ok(ApiResponse<string>.SuccessResponse(token, "Login successful"));
		}
	}
}
