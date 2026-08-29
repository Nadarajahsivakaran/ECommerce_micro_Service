using AutoMapper;
using ECommerce.Caching;
using ECommerce.Models;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Infrastructure.IRepository;
using ProductApi.Models;
using ProductApi.Models.DTO;


namespace ProductApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]

	public class ProductController : ControllerBase
	{
		private readonly IProductRepository _productRepository;
		private readonly IMapper _mapper;
		private readonly ILogger<ProductController> _logger;
		private readonly ICacheService _cache;
		private const string AllProductsCacheKey = "products:all";
		private static string ProductCacheKey(Guid id) => $"product:{id}";
		private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

		public ProductController(IProductRepository productRepository, IMapper mapper, ILogger<ProductController> logger, ICacheService cache)
		{
			_productRepository = productRepository;
			_mapper = mapper;
			_logger = logger;
			_cache = cache;
		}

		#region GetAll
		[HttpGet(nameof(GetAll))]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetAll()
		{
			IEnumerable<ProductDto> result = await _cache.GetOrCreateAsync(
				AllProductsCacheKey,
				async () =>
				{
					IEnumerable<Product> products = await _productRepository.GetAllAsync(null, p => p.Category);
					return _mapper.Map<IEnumerable<ProductDto>>(products);
				},
				CacheDuration);

			if (result == null || !result.Any())
			{
				_logger.LogWarning("GetAll called but no products found.");
				return NotFound(ApiResponse<IEnumerable<ProductDto>>.FailResponse(
					error: "ProductsNotFound",
					message: "No products available",
					statusCode: 404
				));
			}

			return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(result, "Products retrieved successfully"));
		}
		#endregion

		#region GetById/{id}
		[HttpGet(nameof(GetById) + "/{id:guid}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(Guid id)
		{
			if (id == Guid.Empty)
			{
				_logger.LogWarning("GetById called with empty GUID.");
				return BadRequest(ApiResponse<string>.FailResponse(
					error: "InvalidId",
					message: "ID cannot be empty or 0"
				));
			}

			ProductDto? result = await _cache.GetOrCreateAsync(
				ProductCacheKey(id),
				async () =>
				{
					Product product = await _productRepository.FindSingleAsync(p => p.Id == id, p => p.Category);
					return product is null ? null : _mapper.Map<ProductDto>(product);
				},
				CacheDuration);

			if (result == null)
			{
				_logger.LogWarning("GetById: Product not found for ID {ProductId}.", id);
				return NotFound(ApiResponse<string>.FailResponse(
						error: "ProductNotFound",
						message: "No product exists with the given ID",
						statusCode: 404
					));
			}

			return Ok(ApiResponse<ProductDto>.SuccessResponse(
				result,
				"Product retrieved successfully"
			));
		}
		#endregion

		#region Create
		[HttpPost(nameof(Create))]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<ApiResponse<ProductDto>>> Create([FromBody] ProductCreateDto dto)
		{
			if (dto == null)
			{
				_logger.LogWarning("Create Product called with null payload.");
				return BadRequest(ApiResponse<string>.FailResponse(
					error: "InvalidProductData",
					message: "Product payload cannot be null",
					statusCode: 400
				));
			}

			if (!ModelState.IsValid)
			{
				var errors = ModelState.Values
					.SelectMany(v => v.Errors)
					.Select(e => e.ErrorMessage)
					.ToList();

				_logger.LogWarning("Create Product validation failed: {@Errors}", errors);

				return BadRequest(ApiResponse<List<string>>.FailResponse(
					error: "ValidationFailed",
					message: "Invalid product data",
					data: errors,
					statusCode: 400
				));
			}

			Product product = _mapper.Map<Product>(dto);
			await _productRepository.AddAsync(product);
			await _cache.RemoveAsync(AllProductsCacheKey);

			ProductDto result = _mapper.Map<ProductDto>(product);

			return StatusCode(StatusCodes.Status201Created,
				ApiResponse<ProductDto>.SuccessResponse(
					result,
					"Product created successfully",
					statusCode: 201
				));
		}
		#endregion

		#region Update/{id}
		[HttpPut(nameof(Update) + "/{id:guid}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<ApiResponse<ProductDto>>> Update(Guid id, [FromBody] ProductCreateDto dto)
		{

			if (id == Guid.Empty)
			{
				_logger.LogWarning("Update Product called with empty GUID.");
				return BadRequest(ApiResponse<string>.FailResponse(
					error: "InvalidId",
					message: "ID cannot be empty or 0"
				));
			}

			if (dto == null)
			{
				_logger.LogWarning("Update Product called with null payload.");
				return BadRequest(ApiResponse<string>.FailResponse(
					error: "InvalidProductData",
					message: "Product payload cannot be null"
				));
			}

			if (!ModelState.IsValid)
			{
				var errors = ModelState.Values
					.SelectMany(v => v.Errors)
					.Select(e => e.ErrorMessage)
					.ToList();

				_logger.LogWarning("Update Product validation failed: {@Errors}", errors);
				return BadRequest(ApiResponse<List<string>>.FailResponse(
					error: "ValidationFailed",
					data: errors,
					message: "Invalid product data"
				));
			}

			Product product = await _productRepository.FindSingleAsync(
				p => p.Id == id
			);

			if (product == null)
			{
				_logger.LogWarning("Update Product failed. Product not found for ID {ProductId}.", id);
				return NotFound(ApiResponse<string>.FailResponse(
					error: "ProductNotFound",
					message: "No product exists with the given ID",
					statusCode: 404
				));
			}

			_mapper.Map(dto, product);
			await _productRepository.Update(product);

			await _cache.RemoveAsync(ProductCacheKey(id));    
			await _cache.RemoveAsync(AllProductsCacheKey);

			ProductDto result = _mapper.Map<ProductDto>(product);

			return Ok(ApiResponse<ProductDto>.SuccessResponse(
				result,
				"Product updated successfully"
			));

		}
		#endregion

	}
}
