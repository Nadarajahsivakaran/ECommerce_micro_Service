using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProductApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TestController : ControllerBase
	{
		[HttpGet("Test")]
		public IActionResult Test()
		{
			return Ok("Product API is running successfully!");
		}
	}
}
