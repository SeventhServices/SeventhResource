using Microsoft.AspNetCore.Mvc;
using Seventh.Resource.Common.Helpers;

namespace Seventh.Resource.Api.Controllers
{
    [ApiController]
    [Route("{Controller}")]
    public class InfoController : ControllerBase
    {

        [HttpGet("File/{FileName}")]
        public IActionResult GetFileName(string fileName)
        {
            return Ok(new
            {
                HashedFileName = FileNameConverter.ToWithHashName(fileName)
            });
        }
    }
}