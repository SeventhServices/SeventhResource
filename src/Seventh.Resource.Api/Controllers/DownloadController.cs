using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Seventh.Resource.Common.Helpers;
using Seventh.Resource.Services;

namespace Seventh.Resource.Api.Controllers
{
    [ApiController]
    [Route("{Controller}")]
    public class DownloadController : Controller
    {
        private readonly OptionService _option;
        private readonly HttpClient _client;

        public DownloadController(OptionService option,IHttpClientFactory factory)
        {
            _option = option;
            _client = factory.CreateClient("Default");
        }

        [HttpGet("Option")]
        public IActionResult Index()
        {
            return Ok(_option);
        }


        //[Route("{Url}")]

        //public IActionResult TryDownload(string url)
        //{
        //    var byteArray = await _client.GetByteArrayAsync(url);
        //    File.WriteAllBytesAsync(Path.Combine());
        //}
    }

}