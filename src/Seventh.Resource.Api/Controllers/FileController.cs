using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Seventh.Core.Dto.Request.Resource;
using Seventh.Core.Dto.Response.Resource;
using Seventh.Core.Services;
using Seventh.Resource.Common.Entities;
using Seventh.Resource.Services;

namespace Seventh.Resource.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : Controller
    {
        private readonly SevenResourceService _resourceService;
        private readonly AssetDownloadService _downloadService;

        public FileController(SevenResourceService resourceService, AssetDownloadService downloadService)
        {
            _resourceService = resourceService;
            _downloadService = downloadService;
        }

        [HttpPost("Download")]
        [ResponseCache( Duration = 1800 )]
        public async Task<ActionResult<IEnumerable<DownloadFileDto>>> TryDownloadFiles(IEnumerable<TryDownloadFileDto> dtoList)
        {
            var downloadFiles = new List<DownloadFileDto>();
            foreach (var dto in dtoList)
            {
                bool result; AssetFileInfo info;
                if (dto.Revision != null)
                {
                    (result, info) =
                        await _downloadService.TryDownloadAtRevisionAndSortAsync(
                            dto.FileName, (int)dto.Revision,dto.NeedHash == true);
                }
                else
                {
                    (result, info) =
                        await _downloadService.TryDownloadAtMirrorAndSortAsync(
                            dto.FileName, dto.NeedHash == true);
                }

                if (!result)
                {
                    downloadFiles.Add(new DownloadFileDto
                    {
                        Result = false,
                        FileName = dto.FileName,
                    });
                    continue;
                }

                var downloadFileDto = 
                    info.BuildAdapter()
                        .AddParameters("baseUrl",_resourceService.BaseUrl)
                        .AdaptToType<DownloadFileDto>();
                downloadFileDto.Result = true;
                downloadFiles.Add(downloadFileDto);
            }
            return Ok(downloadFiles);
        }

        [HttpGet("Download/{FileName}")]
        [ResponseCache( Duration = 120 )]
        public async Task<ActionResult<DownloadFileDto>> TryGetDownloadFile(
            [RegularExpression("^.*\\..*$")] [Required] string fileName,
            [FromQuery] TryGetDownloadFileDto dto)
        {
            bool result; AssetFileInfo info;

            if (dto.Revision != null)
            {
                (result, info) =
                    await _downloadService.TryDownloadAtRevisionAndSortAsync(
                        fileName, (int)dto.Revision,dto.NeedHash == true);
            }
            else
            {
                (result, info) =
                    await _downloadService.TryDownloadAtMirrorAndSortAsync(
                        fileName, dto.NeedHash == true);
            }

            if (!result)
            {
                return NotFound();
            }

            var downloadFileDto = 
                info.BuildAdapter()
                    .AddParameters("baseUrl",_resourceService.BaseUrl)
                    .AdaptToType<DownloadFileDto>();

            downloadFileDto.Result = true;
            return Ok(downloadFileDto);
        }
    }
}