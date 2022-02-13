namespace Rockstodons.Web.Controllers
{
    using System.Diagnostics;

    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        public IActionResult Get()
        {
            return Ok("It works");
        }
    }
}
