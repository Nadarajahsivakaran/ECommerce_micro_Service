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
	[Authorize(Roles = "Admin,SuperAdmin")]
	public class CategoriesController(ICategoryRepository repo, IMapper mapper) : ControllerBase
	{
		private readonly ICategoryRepository _repo = repo;
		private readonly IMapper _mapper = mapper;

		// ✅ GET: api/categories/GetAll
		[HttpGet(nameof(GetAll))]
		public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetAll()
		{
			var categories = await _repo.GetAllAsync();
			var result = _mapper.Map<IEnumerable<CategoryDto>>(categories);

			return Ok(ApiResponse<IEnumerable<CategoryDto>>
				.SuccessResponse(result, "Categories retrieved successfully"));
		}

		// ✅ GET: api/categories/GetById/{id}
		[HttpGet(nameof(GetById) + "/{id:guid}")]
		public async Task<ActionResult<ApiResponse<CategoryDto>>> GetById(Guid id)
		{
			var category = await _repo.GetByIdAsync(id);

			if (category == null)
			{
				return NotFound(ApiResponse<CategoryDto>.FailResponse(
					error: "CategoryNotFound",
					message: "No category exists with the given ID",
					statusCode: 404
				));
			}

			var result = _mapper.Map<CategoryDto>(category);

			return Ok(ApiResponse<CategoryDto>
				.SuccessResponse(result, "Category retrieved successfully"));
		}

		// ✅ POST: api/categories/Create
		[HttpPost(nameof(Create))]
		public async Task<ActionResult<ApiResponse<CategoryDto>>> Create([FromBody] CategoryCreateDto dto)
		{
			if (dto == null)
			{
				return BadRequest(ApiResponse<CategoryDto>.FailResponse(
					error: "InvalidCategoryData",
					message: "Category payload cannot be null",
					statusCode: 400
				));
			}

			var category = _mapper.Map<Category>(dto);
			await _repo.AddAsync(category);

			var result = _mapper.Map<CategoryDto>(category);

			return StatusCode(201, ApiResponse<CategoryDto>
				.SuccessResponse(result, "Category created successfully", 201));
		}

		// ✅ PUT: api/categories/Update/{id}
		[HttpPut(nameof(Update) + "/{id:guid}")]
		public async Task<ActionResult<ApiResponse<CategoryDto>>> Update(Guid id, [FromBody] CategoryCreateDto dto)
		{
			if (dto == null)
			{
				return BadRequest(ApiResponse<CategoryDto>.FailResponse(
					error: "InvalidCategoryData",
					message: "Category payload cannot be null",
					statusCode: 400
				));
			}

			var category = await _repo.GetByIdAsync(id);
			if (category == null)
			{
				return NotFound(ApiResponse<CategoryDto>.FailResponse(
					error: "CategoryNotFound",
					message: "No category exists with the given ID",
					statusCode: 404
				));
			}

			_mapper.Map(dto, category);
			await _repo.Update(category);

			var result = _mapper.Map<CategoryDto>(category);

			return Ok(ApiResponse<CategoryDto>
				.SuccessResponse(result, "Category updated successfully"));
		}

		//// ✅ DELETE: api/categories/Delete/{id}
		//[HttpDelete(nameof(Delete) + "/{id:guid}")]
		//public async Task<ActionResult<ApiResponse<string>>> Delete(Guid id)
		//{
		//	var category = await _repo.GetByIdAsync(id);

		//	if (category == null)
		//	{
		//		return NotFound(ApiResponse<string>.FailResponse(
		//			error: "CategoryNotFound",
		//			message: "No category exists with the given ID",
		//			statusCode: 404
		//		));
		//	}

		//	await _repo.DeleteA(id);

		//	return Ok(ApiResponse<string>.SuccessResponse(
		//		data: null,
		//		message: "Category deleted successfully",
		//		statusCode: 200
		//	));
		//}
	}
}
