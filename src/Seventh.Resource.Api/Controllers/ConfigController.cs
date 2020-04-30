using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Seventh.Core.Services;
using Seventh.Resource.Api.Core.Dto.Request;
using Seventh.Resource.Api.Core.Dto.Response;
using Seventh.Resource.Common.Entities;
using Seventh.Resource.Services;

namespace Seventh.Resource.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigController : ControllerBase
    {
        private readonly SevenStatusService _statusService;
        private readonly ResourceLocation _location;
        private readonly QueueDownloadService _queueDownloadService;

        public ConfigController(SevenStatusService statusService, 
            ResourceLocation location, QueueDownloadService queueDownloadService)
        {
            _statusService = statusService;
            _location = location;
            _queueDownloadService = queueDownloadService;
        }

        [HttpPut("downloadUrl", Name = nameof(UpdateDownloadUrl))]
        public IActionResult UpdateDownloadUrl(
            [FromBody] UpdateDownloadUrlDto dto)
        {
            _location.DownloadUrl = dto.Url;

            return Ok(new RefreshedDownloadUrlDto
            {
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
                return NotFound();
            }

            _location.DownloadUrl = info.AssetVersion.DownloadUrl;

            return Ok(new RefreshedDownloadUrlDto
            {
                NowDownloadUrl = _location.DownloadUrl
            });
        }

    }
}