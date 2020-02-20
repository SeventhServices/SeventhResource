using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Seventh.Resource.Services;

namespace Seventh.Resource.Api.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        private readonly FileWatcherService _watcher;

        public HomeController(FileWatcherService watcher)
        {
            _watcher = watcher;
        }
        public IActionResult Index()
        {
            _watcher.StartWatch();
            return new JsonResult(new
            {
                Message = "Welcome to 7th(seventh) resource api."
            });
        }
    }
}