using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Seventh.Resource.Services;

namespace Seventh.Resource.Api.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        public IActionResult Index()
        {
            return new JsonResult(new
            {
                Message = "Welcome to 7th(seventh) resource api."
            });
        }
    }
}