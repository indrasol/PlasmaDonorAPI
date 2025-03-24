using Microsoft.AspNetCore.Mvc;

namespace NewPlasmaDonorsAPI.Controllers
{
    [Route("api/health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet("check")]
        public IActionResult Check()
        {
            return Ok("running");
        }
    }
}
