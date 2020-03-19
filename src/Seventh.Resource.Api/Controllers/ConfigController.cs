using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Seventh.Core.Dto.Request.Resource;
using Seventh.Core.Dto.Response.Resource;
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

        [HttpPut("downloadUrl", Name = nameof(UpdateDownloadUrl))]
        public IActionResult UpdateDownloadUrl(
            [FromBody] UpdateDownloadUrlDto dto)
        {
            _location.DownloadUrl = dto.Url;

            return Ok(new RefreshedDownloadUrlDto
            {
                Result = true,
                NowDownloadUrl = _location.DownloadUrl
            });
        }

        [HttpPost("downloadUrl", Name = nameof(CheckAndUpdateDownloadUrl))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CheckAndUpdateDownloadUrl()
        {
            var info = await _statusService.TryGetVersionInfoAsync();

            if (info == null)
            {
                return NotFound(new RefreshedDownloadUrlDto
                {
                    Result = false,
                    NowDownloadUrl = _location.DownloadUrl
                });
            }

            _location.DownloadUrl = info.AssetVersion.DownloadUrl;

            return Ok(new RefreshedDownloadUrlDto
            {
                Result = true,
                NowDownloadUrl = _location.DownloadUrl
            });
        }
    }
}