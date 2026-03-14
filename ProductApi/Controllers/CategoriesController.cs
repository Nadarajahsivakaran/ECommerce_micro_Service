using AutoMapper;
using ECommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Infrastructure.IRepository;
using ProductApi.Models;
using ProductApi.Models.DTO;

namespace ProductApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CategoriesController : ControllerBase
	{
		private readonly ICategoryRepository _repo;
		private readonly IMapper _mapper;
		private readonly ILogger<CategoriesController> _logger;

		public CategoriesController(ICategoryRepository repo, IMapper mapper, ILogger<CategoriesController> logger)
		{
			_repo = repo;
			_mapper = mapper;
			_logger = logger;
		}

		#region GetAll
		[HttpGet("GetAll")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetAll()
		{
			IEnumerable<Category> categories = await _repo.GetAllAsync();
			IEnumerable<CategoryDto> result = _mapper.Map<IEnumerable<CategoryDto>>(categories);
			return Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResponse(result, "Categories retrieved successfully"));
		}
		#endregion

		#region GetById/{id}
		[HttpGet(nameof(GetById) + "/{id:guid}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<ApiResponse<CategoryDto>>> GetById(Guid id)
		{
			if (id == Guid.Empty)
			{
				_logger.LogWarning("GetById called with empty GUID");
				return BadRequest(ApiResponse<string>.FailResponse("InvalidId", "ID cannot be empty or 0"));
			}

			Category? category = await _repo.GetByIdAsync(id);
			if (category == null)
			{
				_logger.LogWarning("Category not found with Id {CategoryId}", id);
				return NotFound(ApiResponse<string>.FailResponse("CategoryNotFound", "No category exists with the given ID"));
			}

			CategoryDto result = _mapper.Map<CategoryDto>(category);
			return Ok(ApiResponse<CategoryDto>.SuccessResponse(result, "Category retrieved successfully"));
		}
		#endregion

		#region Create
		[HttpPost("Create")]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
		{
			if (dto == null)
			{
				_logger.LogWarning("Create called with null payload");
				return BadRequest(ApiResponse<string>.FailResponse("InvalidData", "Category data cannot be null"));
			}

			if (!ModelState.IsValid)
			{
				List<string> errors = [.. ModelState.Values
								.SelectMany(v => v.Errors)
								.Select(e => e.ErrorMessage)];
				_logger.LogWarning("Model validation failed: {Errors}", string.Join("; ", errors));
				return BadRequest(ApiResponse<List<string>>.FailResponse("ValidationFailed", "Invalid category data", errors));
			}

			Category category = _mapper.Map<Category>(dto);
			await _repo.AddAsync(category);
			CategoryDto categoryDto = _mapper.Map<CategoryDto>(category);

			return StatusCode(StatusCodes.Status201Created,
				ApiResponse<CategoryDto>.SuccessResponse(categoryDto, "Category created successfully"));
		}
		#endregion

		#region Update
		[HttpPut(nameof(Update) + "/{id:guid}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<ApiResponse<CategoryDto>>> Update(Guid id, [FromBody] CategoryCreateDto dto)
		{
			if (id == Guid.Empty)
			{
				_logger.LogWarning("Update called with empty GUID");
				return BadRequest(ApiResponse<string>.FailResponse("InvalidId", message: "ID cannot be empty or 0"));
			}

			if (dto == null)
			{
				_logger.LogWarning("Update called with null payload");
				return BadRequest(ApiResponse<string>.FailResponse("InvalidCategoryData", "Category payload cannot be null"));
			}

			if (!ModelState.IsValid)
			{
				List<string> errors = [.. ModelState.Values
								.SelectMany(v => v.Errors)
								.Select(e => e.ErrorMessage)];
				_logger.LogWarning("Model validation failed: {Errors}", string.Join("; ", errors));
				return BadRequest(ApiResponse<List<string>>.FailResponse("ValidationFailed", "Invalid input data", errors));
			}

			Category? category = await _repo.GetByIdAsync(id);
			if (category == null)
			{
				_logger.LogWarning("Category not found with Id {CategoryId}", id);
				return NotFound(ApiResponse<string>.FailResponse("CategoryNotFound", "No category exists with the given ID"));
			}

			_mapper.Map(dto, category);
			await _repo.Update(category);

			CategoryDto result = _mapper.Map<CategoryDto>(category);
			return Ok(ApiResponse<CategoryDto>.SuccessResponse(result, "Category updated successfully"));
		}
		#endregion
	}
}