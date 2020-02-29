using Microsoft.AspNetCore.Mvc;
using Seventh.Resource.Common.Helpers;

namespace Seventh.Resource.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InfoController : ControllerBase
    {
        [HttpGet("File/{FileName}")]
        public IActionResult GetFileInfo(string fileName)
        {
            return Ok(new
            {
                HashedFileName = FileNameConverter.ToWithHashName(fileName)
            });
        }
    }
}