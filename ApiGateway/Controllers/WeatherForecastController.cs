using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Policy = "AdminOnly")]
	public class WeatherForecastController : ControllerBase
	{
		[HttpGet]
		public IActionResult Get()
		{
			return Ok("get it");
		}
	}
}
