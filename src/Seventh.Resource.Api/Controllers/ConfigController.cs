using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Seventh.Core.Dto.Request.Resource;
using Seventh.Core.Services;
using Seventh.Resource.Services;

namespace Seventh.Resource.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigController : ControllerBase
    {
        private readonly SevenStatusService _statusService;
        private readonly ResourceLocation _location;

        public ConfigController(SevenStatusService statusService,ResourceLocation location, IHttpClientFactory httpClientFactory)
        {
            _statusService = statusService;
            _location = location;
        }

        [HttpPut("{DownloadUrl}")]
        public IActionResult UpdateDownloadUrl([FromBody] UpdateDownloadUrlDto dto)
        {
            _location.DownloadUrl = dto.DownloadUrl;

            return Ok(new
            {
                downloadUrl = _location.DownloadUrl
            });
        }

        [HttpPost("{DownloadUrl}")]
        public async Task<IActionResult> RefreshDownloadUrl()
        {
            var info = await _statusService.TryGetVersionInfoAsync();

            if (info == null)
            {
                return NotFound();
            }

            _location.DownloadUrl = info.AssetVersion.DownloadUrl;

            return Ok(new
            {
                downloadUrl = _location.DownloadUrl
            });
        }
    }
}