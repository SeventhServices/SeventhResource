using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Seventh.Core;
using Seventh.Resource.Api.Dto;
using Seventh.Resource.Services;

namespace Seventh.Resource.Api.Controllers
{
    [ApiController]
    [Route("{Controller}")]
    public class ConfigController : ControllerBase
    {
        private readonly SevenStatusService _statusService;
        private readonly ResourceLocation _location;
        private readonly HttpClient _httpClient;

        public ConfigController(SevenStatusService statusService,ResourceLocation location, IHttpClientFactory httpClientFactory)
        {
            _statusService = statusService;
            _location = location;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost("{DownloadUrl}")]
        public IActionResult UpdateDownloadUrl([FromBody] UpdateDownloadUrlDto dto)
        {
            _location.DownloadUrl = dto.DownloadUrl;

            return Ok(new
            {
                downloadUrl = _location.DownloadUrl
            });
        }

        [HttpPut("{DownloadUrl}")]
        public async Task<IActionResult> RefreshDownloadUrl()
        {
            var info = 
                await _httpClient.GetStringAsync(_statusService.GetVersionInfoUrl());

            return Ok(new
            {
                downloadUrl = _location.DownloadUrl
            });
        }
    }
}