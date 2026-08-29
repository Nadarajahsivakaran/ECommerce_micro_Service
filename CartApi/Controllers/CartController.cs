using CartApi.Infrastructure;
using CartApi.Models;
using CartApi.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace CartApi.Controllers
{
	[ApiController]
	[Route("api/cart")]
	public class CartController : ControllerBase
	{
		private readonly CartService _service;

		public CartController(CartService service)
		{
			_service = service;
		}

		[HttpGet("{userId}")]
		public async Task<IActionResult> GetCart(string userId)
		{
			var cart = await _service.GetCart(userId);
			return Ok(cart);
		}

		[HttpPost("add")]
		public async Task<IActionResult> AddItem(AddItemDto dto)
		{
			var item = new CartItem
			{
				ProductId = dto.ProductId,
				Quantity = dto.Quantity
			};

			var result = await _service.AddItem(dto.UserId, item);
			return Ok(result);
		}

		[HttpDelete("item")]
		public async Task<IActionResult> Remove(string userId, string productId)
		{
			var result = await _service.RemoveItem(userId, productId);
			return Ok(result);
		}

		[HttpDelete("clear/{userId}")]
		public async Task<IActionResult> Clear(string userId)
		{
			await _service.ClearCart(userId);
			return Ok();
		}
	}
}
