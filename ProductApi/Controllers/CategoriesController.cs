using AutoMapper;
using ECommerce.Models;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Infrastructure.IRepository;
using ProductApi.Models;
using ProductApi.Models.DTO;

namespace ProductApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CategoriesController(ICategoryRepository repo, IMapper mapper) : ControllerBase
	{
		private readonly ICategoryRepository _repo = repo;
		private readonly IMapper _mapper = mapper;

		[HttpGet(nameof(GetAll))]
		public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetAll()
		{
			IEnumerable<Category> categories = await _repo.GetAllAsync();
			IEnumerable<CategoryDto> result = _mapper.Map<IEnumerable<CategoryDto>>(categories);
			return Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResponse(result, "Categories retrieved successfully"));
		}

		[HttpPost("Create")]
		public async Task<IActionResult> Create(CategoryCreateDto dto)
		{
			if (dto == null)
				return BadRequest("Invalid category data");

			Category category = _mapper.Map<Category>(dto);
			await _repo.AddAsync(category);
			CategoryDto categoryDto = _mapper.Map<CategoryDto>(category);
			return Ok(ApiResponse<CategoryDto>.SuccessResponse(categoryDto, "Category created successfully", 201));
		}
	}
}
