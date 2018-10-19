using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WidgetApi.Models;

namespace WidgetApi.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class WidgetController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await Task.Delay(100); // simulate latency
            var items = new List<Widget>()
            {
                new Widget { ID = 1, Name = "Cog", Shape = "Square" },
                new Widget { ID = 2, Name = "Gear", Shape = "Round" },
                new Widget { ID = 3, Name = "Sprocket", Shape = "Octagonal" },
                new Widget { ID = 4, Name = "Pinion", Shape = "Triangular" }
            };

            return Ok(items);
        }
    }
}
