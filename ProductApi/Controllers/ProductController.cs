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
	public class ProductController(IProductRepository productRepository, IMapper mapper) : ControllerBase
	{
		private readonly IProductRepository _productRepository = productRepository;
		private readonly IMapper _mapper = mapper;

		[HttpGet(nameof(GetAll))]
		public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetAll()
		{
			IEnumerable<Product> products = await _productRepository.GetAllAsync(null, p => p.Category);
			IEnumerable<ProductDto> result = _mapper.Map<IEnumerable<ProductDto>>(products);
			return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(result, "Products retrieved successfully"));
		}
	}
}
