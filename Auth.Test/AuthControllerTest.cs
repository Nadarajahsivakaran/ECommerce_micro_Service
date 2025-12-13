using AuthApi.Controllers;
using AuthApi.Data.IRepository;
using AuthApi.Models;
using AuthApi.Models.DTO;
using AutoMapper;
using ECommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Auth.Test
{
    public class AuthControllerTest
    {
		private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
		private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
		private readonly Mock<IMapper> _mockMapper;
		private readonly Mock<IAuthService> _mockAuthService;
		private readonly AuthController _controller;

		public AuthControllerTest()
        {
			// Mock UserManager
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			_mockUserManager = new Mock<UserManager<ApplicationUser>>(
				userStore.Object, null, null, null, null, null, null, null, null
			);

			// Mock RoleManager
			var roleStore = new Mock<IRoleStore<IdentityRole>>();
			_mockRoleManager = new Mock<RoleManager<IdentityRole>>(
				roleStore.Object, null, null, null, null
			);

			// Mock Mapper
			_mockMapper = new Mock<IMapper>();

			//mock Service
			_mockAuthService = new Mock<IAuthService>();


			// Instantiate controller
			_controller = new AuthController(
				_mockUserManager.Object,
				_mockRoleManager.Object,
				_mockAuthService.Object,
				_mockMapper.Object
			);
		}

		//Test 1: Invalid ModelState
		[Fact]
		public async Task Register_InvalidModelState_ReturnsBadRequest()
        {
			// Arrange: add an error to simulate invalid model
			_controller.ModelState.AddModelError("Email", "Email is required");
			RegisterDto dto = new ();

			// Act
			IActionResult result = await _controller.Register(dto);

			// Assert
			var badRequest = Assert.IsType<BadRequestObjectResult>(result);
			var response = Assert.IsType<ApiResponse<RegisterDto>>(badRequest.Value);
			Assert.Contains("Email is required", response.Error);
			Assert.Equal("Validation failed", response.Message);
		}

		// Test 2: User creation fails
		[Fact]
		public async Task Register_UserCreationFails_ReturnsBadRequest()
		{
			// Arrange
			RegisterDto dto = new() { Email = "test@test.com", Password = "Password123" };
			var user = new ApplicationUser();

			_mockMapper.Setup(m => m.Map<ApplicationUser>(dto)).Returns(user);

			// Simulate failure in creating user
			_mockUserManager.Setup(um => um.CreateAsync(user, dto.Password))
				.ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User creation failed" }));

			// Act
			var result = await _controller.Register(dto);

			// Assert
			var badRequest = Assert.IsType<BadRequestObjectResult>(result);
			var response = Assert.IsType<ApiResponse<RegisterDto>>(badRequest.Value);
			Assert.Contains("User creation failed", response.Error);
			Assert.Equal("Registration failed", response.Message);
		}

		// Test 3: Role does not exist, needs creation
		[Fact]
		public async Task Register_RoleDoesNotExist_CreatesRoleAndReturnsOk()
		{
			// Arrange
			RegisterDto dto = new () { Email = "test@test.com", Password = "Password123" };
			ApplicationUser user = new ();
			RegisterResponseDto responseDto = new () { Email = "test@test.com" };

			_mockMapper.Setup(m => m.Map<ApplicationUser>(dto)).Returns(user);
			_mockUserManager.Setup(um => um.CreateAsync(user, dto.Password)).ReturnsAsync(IdentityResult.Success);

			// Simulate role does not exist
			_mockRoleManager.Setup(rm => rm.RoleExistsAsync("User")).ReturnsAsync(false);
			_mockRoleManager.Setup(rm => rm.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);
			_mockUserManager.Setup(um => um.AddToRoleAsync(user, "User")).ReturnsAsync(IdentityResult.Success);
			_mockMapper.Setup(m => m.Map<RegisterResponseDto>(user)).Returns(responseDto);

			// Act
			IActionResult result = await _controller.Register(dto);

			// Assert
			OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
			ApiResponse<RegisterResponseDto> response = Assert.IsType<ApiResponse<RegisterResponseDto>>(okResult.Value);
			Assert.Equal(201, response.StatusCode);
			Assert.Equal("User registered successfully", response.Message);
			Assert.Equal("test@test.com", response.Data.Email);
		}

		// Test 4: Role exists, just assign role
		[Fact]
		public async Task Register_RoleExists_AssignsRoleAndReturnsOk()
		{
			// Arrange
			RegisterDto dto = new () { Email = "test2@test.com", Password = "Password123" };
			ApplicationUser user = new ();
			RegisterResponseDto responseDto = new () { Email = "test2@test.com" };

			_mockMapper.Setup(m => m.Map<ApplicationUser>(dto)).Returns(user);
			_mockUserManager.Setup(um => um.CreateAsync(user, dto.Password)).ReturnsAsync(IdentityResult.Success);

			// Simulate role already exists
			_mockRoleManager.Setup(rm => rm.RoleExistsAsync("User")).ReturnsAsync(true);
			_mockUserManager.Setup(um => um.AddToRoleAsync(user, "User")).ReturnsAsync(IdentityResult.Success);
			_mockMapper.Setup(m => m.Map<RegisterResponseDto>(user)).Returns(responseDto);

			// Act
			IActionResult result = await _controller.Register(dto);

			// Assert
			OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
			ApiResponse<RegisterResponseDto> response = Assert.IsType<ApiResponse<RegisterResponseDto>>(okResult.Value);
			Assert.Equal(201, response.StatusCode);
			Assert.Equal("User registered successfully", response.Message);
			Assert.Equal("test2@test.com", response.Data.Email);
		}

		// Test 5: Assign role fails
		//[Fact]
		//public async Task Register_AssignRoleFails_ReturnsBadRequest()
		//{
		//	// Arrange
		//	RegisterDto dto = new () { Email = "fail@test.com", Password = "Password123" };
		//	ApplicationUser user = new ();

		//	_mockMapper.Setup(m => m.Map<ApplicationUser>(dto)).Returns(user);
		//	_mockUserManager.Setup(um => um.CreateAsync(user, dto.Password)).ReturnsAsync(IdentityResult.Success);
		//	_mockRoleManager.Setup(rm => rm.RoleExistsAsync("User")).ReturnsAsync(true);

		//	// Simulate failure adding role
		//	_mockUserManager.Setup(um => um.AddToRoleAsync(user, "User"))
		//		.ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Role assignment failed" }));

		//	// Act
		//	IActionResult result = await _controller.Register(dto);

		//	// Assert
		//	BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
		//	ApiResponse<RegisterDto> response = Assert.IsType<ApiResponse<RegisterDto>>(badRequest.Value);
		//	Assert.Contains("Role assignment failed", response.Error);
		//}

	}
}
