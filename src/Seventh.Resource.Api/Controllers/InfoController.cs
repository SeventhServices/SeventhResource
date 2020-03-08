using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Seventh.Core.Dto.Response.Resource;
using Seventh.Resource.Common.Helpers;
using Seventh.Resource.Services;
using System.Threading.Tasks;
using Mapster;
using Seventh.Core.Dto.Request.Resource;
using Seventh.Core.Services;
using System.Collections.Generic;
using Seventh.Core.Dto.Response.Status;
using System.Linq;

namespace Seventh.Resource.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InfoController : ControllerBase
    {
        private readonly SevenResourceService _resourceService;
        private readonly AssetInfoService _infoService;

        public InfoController(SevenResourceService resourceService,AssetInfoService infoService)
        {
            _resourceService = resourceService;
            _infoService = infoService;
        }

        [ResponseCache(Duration = 120)]
        [HttpHead("Files")]
        [HttpGet("Files")]
        public async Task<ActionResult<GetFileResultDto>> 
            GetFileInfos([FromQuery] GetFileDtoParams dto)
        {
            return NotFound();
        }

        [ResponseCache(Duration = 300)]
        [HttpGet("Revision/{Revision}")]
        [HttpHead("Revision/{Revision}")]
        public async Task<ActionResult<IEnumerable<GetFileResultDto>>> 
            GetFileInfoByRev([Required] int revision)
        {
            var infos = await _infoService
                .TryGetFileInfoByRevAsync(revision);

            if (infos == null)
            {
                return NotFound();
            }

            var infoDtoList = infos.Select(info 
                => new GetFileResultDto
                {
                    IsFileExist = info.FileSize != 0,
                    IsRealFileExist = info.RealFileSize != 0,
                    FileInfo = info.BuildAdapter()
                        .AddParameters("baseUrl", _resourceService.BaseUrl)
                        .AdaptToType<AssetFileInfoDto>()
                });

            return Ok(infoDtoList);
        }

        [ResponseCache(Duration = 120)]
        [HttpHead("File/{FileName}")]
        [HttpGet("File/{FileName}")]
        public async Task<ActionResult<GetFileResultDto>> GetFileInfo(
            [RegularExpression("^.*\\..*$")] [Required] string fileName, 
            [FromQuery] GetFileDtoParams dto)
        {
            var info = await _infoService.TryGetFileInfoAsync(
                fileName,dto.Revision, dto.NeedHash);

            var result = new GetFileResultDto
            {
                IsFileExist = info.FileSize != 0,
                IsRealFileExist = info.RealFileSize != 0,
                FileInfo = info.BuildAdapter()
                    .AddParameters("baseUrl", _resourceService.BaseUrl)
                    .AdaptToType<AssetFileInfoDto>()
            };

            return Ok(result);
        }
    }
}