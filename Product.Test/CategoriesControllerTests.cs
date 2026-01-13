
using ECommerce.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductApi.Controllers;
using ProductApi.Infrastructure.IRepository;
using ProductApi.Models;
using ProductApi.Models.DTO;
using Moq;
using AutoMapper;

namespace Product.Test
{

	public class CategoriesControllerTests
	{
		private readonly Mock<ICategoryRepository> _repoMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<ILogger<CategoriesController>> _loggerMock;
		private readonly CategoriesController _controller;

		public CategoriesControllerTests()
		{
			_repoMock = new Mock<ICategoryRepository>();
			_mapperMock = new Mock<IMapper>();
			_loggerMock = new Mock<ILogger<CategoriesController>>();

			_controller = new CategoriesController(
				_repoMock.Object,
				_mapperMock.Object,
				_loggerMock.Object
			);
		}

		#region GetAll

		[Fact]
		public async Task GetAll_ShouldReturn200Ok_WithCategoryList()
		{
			// Arrange
			var categories = new List<Category>
			{
				new Category { Id = Guid.NewGuid(), Name = "Electronics" }
			};

			var categoryDtos = new List<CategoryDto>
			{
				new CategoryDto { Id = categories[0].Id, Name = "Electronics" }
			};

			_repoMock.Setup(r => r.GetAllAsync())
					 .ReturnsAsync(categories);

			_mapperMock.Setup(m => m.Map<IEnumerable<CategoryDto>>(categories))
					   .Returns(categoryDtos);

			// Act
			var result = await _controller.GetAll();

			// Assert
			var okResult = result.Result as OkObjectResult;
			okResult.Should().NotBeNull();
			okResult!.StatusCode.Should().Be(200);

			var response = okResult.Value as ApiResponse<IEnumerable<CategoryDto>>;
			response.Should().NotBeNull();
			response!.Success.Should().BeTrue();
			response.Data.Should().HaveCount(1);
		}

		#endregion

		#region GetById

		[Fact]
		public async Task GetById_ShouldReturn400_WhenIdIsEmpty()
		{
			// Act
			var result = await _controller.GetById(Guid.Empty);

			// Assert
			var badRequest = result.Result as BadRequestObjectResult;
			badRequest.Should().NotBeNull();
			badRequest!.StatusCode.Should().Be(400);
		}

		[Fact]
		public async Task GetById_ShouldReturn404_WhenCategoryNotFound()
		{
			// Arrange
			var id = Guid.NewGuid();
			_repoMock.Setup(r => r.GetByIdAsync(id))
					 .ReturnsAsync((Category?)null);

			// Act
			var result = await _controller.GetById(id);

			// Assert
			var notFound = result.Result as NotFoundObjectResult;
			notFound.Should().NotBeNull();
			notFound!.StatusCode.Should().Be(404);
		}

		[Fact]
		public async Task GetById_ShouldReturn200_WhenCategoryExists()
		{
			// Arrange
			var id = Guid.NewGuid();
			var category = new Category { Id = id, Name = "Books" };
			var dto = new CategoryDto { Id = id, Name = "Books" };

			_repoMock.Setup(r => r.GetByIdAsync(id))
					 .ReturnsAsync(category);

			_mapperMock.Setup(m => m.Map<CategoryDto>(category))
					   .Returns(dto);

			// Act
			var result = await _controller.GetById(id);

			// Assert
			var okResult = result.Result as OkObjectResult;
			okResult.Should().NotBeNull();
			okResult!.StatusCode.Should().Be(200);
		}

		#endregion

		#region Create

		[Fact]
		public async Task Create_ShouldReturn400_WhenDtoIsNull()
		{
			// Act
			var result = await _controller.Create(null!);

			// Assert
			var badRequest = result as BadRequestObjectResult;
			badRequest.Should().NotBeNull();
			badRequest!.StatusCode.Should().Be(400);
		}

		[Fact]
		public async Task Create_ShouldReturn201_WhenValidDto()
		{
			// Arrange
			var dto = new CategoryCreateDto { Name = "Fashion" };
			var category = new Category { Id = Guid.NewGuid(), Name = "Fashion" };
			var categoryDto = new CategoryDto { Id = category.Id, Name = "Fashion" };

			_mapperMock.Setup(m => m.Map<Category>(dto))
					   .Returns(category);

			_mapperMock.Setup(m => m.Map<CategoryDto>(category))
					   .Returns(categoryDto);

			_repoMock.Setup(r => r.AddAsync(category))
					 .Returns(Task.CompletedTask);

			// Act
			var result = await _controller.Create(dto);

			// Assert
			var createdResult = result as ObjectResult;
			createdResult.Should().NotBeNull();
			createdResult!.StatusCode.Should().Be(201);
		}

		#endregion

		#region Update

		[Fact]
		public async Task Update_ShouldReturn404_WhenCategoryNotFound()
		{
			// Arrange
			var id = Guid.NewGuid();
			var dto = new CategoryCreateDto { Name = "Updated" };

			_repoMock.Setup(r => r.GetByIdAsync(id))
					 .ReturnsAsync((Category?)null);

			// Act
			var result = await _controller.Update(id, dto);

			// Assert
			var notFound = result.Result as NotFoundObjectResult;
			notFound.Should().NotBeNull();
			notFound!.StatusCode.Should().Be(404);
		}

		[Fact]
		public async Task Update_ShouldReturn200_WhenUpdateSucceeds()
		{
			// Arrange
			var id = Guid.NewGuid();
			var dto = new CategoryCreateDto { Name = "Updated" };
			var category = new Category { Id = id, Name = "Old" };
			var categoryDto = new CategoryDto { Id = id, Name = "Updated" };

			_repoMock.Setup(r => r.GetByIdAsync(id))
					 .ReturnsAsync(category);

			_repoMock.Setup(r => r.Update(category))
					 .Returns(Task.CompletedTask);

			_mapperMock.Setup(m => m.Map(dto, category));
			_mapperMock.Setup(m => m.Map<CategoryDto>(category))
					   .Returns(categoryDto);

			// Act
			var result = await _controller.Update(id, dto);

			// Assert
			var okResult = result.Result as OkObjectResult;
			okResult.Should().NotBeNull();
			okResult!.StatusCode.Should().Be(200);
		}

		#endregion
	}
}

