using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Seventh.Core;
using Seventh.Resource.Api.Dto;
using Seventh.Resource.Common.Helpers;
using Seventh.Resource.Services;

namespace Seventh.Resource.Api.Controllers
{
    [ApiController]
    [Route("{Controller}")]
    public class FileController : Controller
    {
        private readonly ResourceLocation _option;
        private readonly SevenResourceService _resourceService;
        private readonly AssetDownloadService _downloadService;

        public FileController(ResourceLocation option,SevenResourceService resourceService, AssetDownloadService downloadService)
        {
            _option = option;
            _resourceService = resourceService;
            _downloadService = downloadService;
        }

        [HttpGet("Option")]
        public IActionResult Index()
        {
            return Ok(_option);
        }

        [HttpGet("{FileName}")]
        public async Task<IActionResult> GetOrDownloadFile(
            [RegularExpression("^.*\\..*$")] [Required] string fileName,
            [FromQuery] GetOrDownloadFileDto dto)
        {
            bool result; string savePath; string sortedSavePath;

            if (dto.Revision != null)
            {
                (result, savePath, sortedSavePath) =
                    await _downloadService.TryDownloadAtRevisionAndSortAsync(
                        fileName, (int)dto.Revision,dto.NeedHash == true);
            }
            else
            {
                (result, savePath, sortedSavePath) =
                    await _downloadService.TryDownloadAtMirrorAndSortAsync(
                        fileName, dto.NeedHash == true);
            }

            if (!result)
            {
                return NotFound();
            }

            return Ok(new
            {
                mirrorUrl = _resourceService.GetDownloadUrl(savePath.Replace(_option.PathOption.RootPath, string.Empty)),
                sortedUrl = _resourceService.GetDownloadUrl(sortedSavePath.Replace(_option.PathOption.RootPath, string.Empty))
            });
        }


    }
}