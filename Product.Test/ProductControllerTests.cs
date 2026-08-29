//using AutoMapper;
//using ECommerce.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using Moq;
//using ProductApi.Controllers;
//using ProductApi.Infrastructure.IRepository;
//using System.Linq.Expressions;


//// Aliases to avoid conflicts
//using ProductCreateDtoModel = ProductApi.Models.DTO.ProductCreateDto;
//using ProductDtoModel = ProductApi.Models.DTO.ProductDto;
//using ProductModel = ProductApi.Models.Product;

//namespace Product.Test
//{
//	public class ProductControllerTests
//	{
//		private readonly Mock<IProductRepository> _repoMock;
//		private readonly Mock<IMapper> _mapperMock;
//		private readonly Mock<ILogger<ProductController>> _loggerMock;
//		private readonly ProductController _controller;

//		public ProductControllerTests()
//		{
//			_repoMock = new Mock<IProductRepository>();
//			_mapperMock = new Mock<IMapper>();
//			_loggerMock = new Mock<ILogger<ProductController>>();

//			_controller = new ProductController(
//				_repoMock.Object,
//				_mapperMock.Object,
//				_loggerMock.Object
//			);
//		}

//		#region GetAll

//		[Fact]
//		public async Task GetAll_ShouldReturn404_WhenNoProductsExist()
//		{
//			_repoMock.Setup(r => r.GetAllAsync(null, It.IsAny<Expression<Func<ProductModel, object>>[]>()))
//					 .ReturnsAsync(new List<ProductModel>());

//			var result = await _controller.GetAll();

//			var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
//			Assert.Equal(404, notFound.StatusCode);
//		}

//		[Fact]
//		public async Task GetAll_ShouldReturn200_WhenProductsExist()
//		{
//			var products = new List<ProductModel>
//			{
//				new ProductModel { Id = Guid.NewGuid(), Name = "Laptop" }
//			};

//			var dtos = new List<ProductDtoModel>
//			{
//				new ProductDtoModel { Id = products[0].Id, Name = "Laptop" }
//			};

//			_repoMock.Setup(r => r.GetAllAsync(null, It.IsAny<Expression<Func<ProductModel, object>>[]>()))
//					 .ReturnsAsync(products);

//			_mapperMock.Setup(m => m.Map<IEnumerable<ProductDtoModel>>(products))
//					   .Returns(dtos);

//			var result = await _controller.GetAll();

//			var okResult = Assert.IsType<OkObjectResult>(result.Result);
//			var response = Assert.IsType<ApiResponse<IEnumerable<ProductDtoModel>>>(okResult.Value);
//			Assert.True(response.Success);
//			Assert.Single(response.Data);
//		}

//		#endregion

//		#region GetById

//		[Fact]
//		public async Task GetById_ShouldReturn400_WhenIdIsEmpty()
//		{
//			var result = await _controller.GetById(Guid.Empty);

//			var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
//			Assert.Equal(400, badRequest.StatusCode);
//		}

//		[Fact]
//		public async Task GetById_ShouldReturn404_WhenProductNotFound()
//		{
//			var id = Guid.NewGuid();

//			_repoMock.Setup(r => r.FindSingleAsync(It.IsAny<Expression<Func<ProductModel, bool>>>(),
//												   It.IsAny<Expression<Func<ProductModel, object>>[]>()))
//					 .ReturnsAsync((ProductModel)null);

//			var result = await _controller.GetById(id);

//			var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
//			Assert.Equal(404, notFound.StatusCode);
//		}

//		[Fact]
//		public async Task GetById_ShouldReturn200_WhenProductExists()
//		{
//			var id = Guid.NewGuid();
//			var product = new ProductModel { Id = id, Name = "Laptop" };
//			var dto = new ProductDtoModel { Id = id, Name = "Laptop" };

//			_repoMock.Setup(r => r.FindSingleAsync(It.IsAny<Expression<Func<ProductModel, bool>>>(),
//												   It.IsAny<Expression<Func<ProductModel, object>>[]>()))
//					 .ReturnsAsync(product);

//			_mapperMock.Setup(m => m.Map<ProductDtoModel>(product)).Returns(dto);

//			var result = await _controller.GetById(id);

//			var okResult = Assert.IsType<OkObjectResult>(result.Result);
//			var response = Assert.IsType<ApiResponse<ProductDtoModel>>(okResult.Value);
//			Assert.True(response.Success);
//			Assert.Equal("Laptop", response.Data.Name);
//		}

//		#endregion

//		#region Create

//		[Fact]
//		public async Task Create_ShouldReturn400_WhenDtoIsNull()
//		{
//			var result = await _controller.Create(null);

//			var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
//			Assert.Equal(400, badRequest.StatusCode);
//		}

//		[Fact]
//		public async Task Create_ShouldReturn201_WhenValidDto()
//		{
//			var dto = new ProductCreateDtoModel { Name = "Phone" };
//			var product = new ProductModel { Id = Guid.NewGuid(), Name = "Phone" };
//			var productDto = new ProductDtoModel { Id = product.Id, Name = "Phone" };

//			_mapperMock.Setup(m => m.Map<ProductModel>(dto)).Returns(product);
//			_mapperMock.Setup(m => m.Map<ProductDtoModel>(product)).Returns(productDto);
//			_repoMock.Setup(r => r.AddAsync(product)).Returns(Task.CompletedTask);

//			var result = await _controller.Create(dto);

//			var createdResult = Assert.IsType<ObjectResult>(result.Result);
//			Assert.Equal(201, createdResult.StatusCode);

//			var response = Assert.IsType<ApiResponse<ProductDtoModel>>(createdResult.Value);
//			Assert.True(response.Success);
//			Assert.Equal("Phone", response.Data.Name);
//		}

//		#endregion

//		#region Update

//		[Fact]
//		public async Task Update_ShouldReturn400_WhenIdIsEmpty()
//		{
//			var dto = new ProductCreateDtoModel { Name = "Tablet" };
//			var result = await _controller.Update(Guid.Empty, dto);

//			var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
//			Assert.Equal(400, badRequest.StatusCode);
//		}

//		[Fact]
//		public async Task Update_ShouldReturn404_WhenProductNotFound()
//		{
//			var id = Guid.NewGuid();
//			var dto = new ProductCreateDtoModel { Name = "Tablet" };

//			_repoMock.Setup(r => r.FindSingleAsync(It.IsAny<Expression<Func<ProductModel, bool>>>(),
//												   It.IsAny<Expression<Func<ProductModel, object>>[]>()))
//					 .ReturnsAsync((ProductModel)null);

//			var result = await _controller.Update(id, dto);

//			var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
//			Assert.Equal(404, notFound.StatusCode);
//		}

//		[Fact]
//		public async Task Update_ShouldReturn200_WhenUpdateSucceeds()
//		{
//			var id = Guid.NewGuid();
//			var dto = new ProductCreateDtoModel { Name = "Tablet" };
//			var product = new ProductModel { Id = id, Name = "OldTablet" };
//			var productDto = new ProductDtoModel { Id = id, Name = "Tablet" };

//			_repoMock.Setup(r => r.FindSingleAsync(It.IsAny<Expression<Func<ProductModel, bool>>>(),
//												   It.IsAny<Expression<Func<ProductModel, object>>[]>()))
//					 .ReturnsAsync(product);

//			_mapperMock.Setup(m => m.Map(dto, product));
//			_mapperMock.Setup(m => m.Map<ProductDtoModel>(product)).Returns(productDto);
//			_repoMock.Setup(r => r.Update(product)).Returns(Task.CompletedTask);

//			var result = await _controller.Update(id, dto);

//			var okResult = Assert.IsType<OkObjectResult>(result.Result);
//			var response = Assert.IsType<ApiResponse<ProductDtoModel>>(okResult.Value);
//			Assert.True(response.Success);
//			Assert.Equal("Tablet", response.Data.Name);
//		}

//		#endregion
//	}
//}
