using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VismaAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProtectedController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            // Your logic to generate random data or any relevant payload
            var randomData = GenerateRandomData();

            return Ok(randomData);
        }

        private string GenerateRandomData()
        {
            // Replace this with your logic to generate random data
            return $"{Guid.NewGuid()}";
        }
    }
}
