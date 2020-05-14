using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Seventh.Core.Services;
using Seventh.Core.Utilities;
using Seventh.Resource.Api.Core.Dto.Request;
using Seventh.Resource.Api.Core.Dto.Response;
using Seventh.Resource.Services;

namespace Seventh.Resource.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InfoController : ControllerBase
    {
        private readonly SeventhResourceService _resourceService;
        private readonly AssetInfoService _infoService;

        public InfoController(SeventhResourceService resourceService, AssetInfoService infoService)
        {
            _resourceService = resourceService;
            _infoService = infoService;
        }

        [ResponseCache(Duration = 10)]
        [HttpHead("classes", Name = nameof(GetAllClasses))]
        [HttpGet("classes", Name = nameof(GetAllClasses))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ICollection<string>>> GetAllClasses()
        {
            var classNames = await _infoService
                .TryGetAllClassNamesAsync();

            if (classNames == null)
            {
                return NotFound();
            }

            return Ok(classNames.Select(c => new
            {
                Name = c.Replace(Path.DirectorySeparatorChar, '/'),
                Url = UrlUtil.MakeFileUrl(string.Concat(
                    _resourceService.BaseUrl, "info/class/"), c)
            }));
        }

        [ResponseCache(Duration = 10)]
        [HttpHead("class/{**className}", Name = nameof(GetFileInfoByClass))]
        [HttpGet("class/{**className}", Name = nameof(GetFileInfoByClass))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ICollection<AssetInfoDto>>>
            GetFileInfoByClass([Required] string className,
            [FromQuery] QueryFileParamsDto queryDto)
        {
            var infos = await _infoService
                .TryGetFileInfoByClassAsync(className);

            if (infos == null)
            {
                return NotFound();
            }

            var infoDtoList = infos.Query(queryDto).Select(info
                => info.BuildAdapter()
                        .AddParameters("baseUrl", _resourceService.BaseUrl)
                        .AdaptToType<AssetFileInfoDto>());

            return Ok(infoDtoList);
        }

        [ResponseCache(Duration = 10)]
        [HttpHead("revision/{revision}", Name = nameof(GetAssetInfoByRev))]
        [HttpGet("revision/{revision}", Name = nameof(GetAssetInfoByRev))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ICollection<AssetInfoDto>>>
            GetAssetInfoByRev([Required] int revision,
            [FromQuery] QueryAssetParamsDto queryDto)
        {
            var infos = await _infoService
                .TryGetAssetInfoByRevAsync(revision);

            if (infos == null)
            {
                return NotFound();
            }

            var infoDtoList = infos.Query(queryDto).Select(info
                => info.BuildAdapter()
                        .AddParameters("baseUrl", _resourceService.BaseUrl)
                        .AdaptToType<AssetInfoDto>());

            return Ok(infoDtoList);
        }

        [ResponseCache(Duration = 120)]
        [HttpHead("file/{fileName}", Name = nameof(GetAssetInfo))]
        [HttpGet("file/{fileName}", Name = nameof(GetAssetInfo))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<AssetInfoDto>> GetAssetInfo(
            [RegularExpression("^.*\\..*$")] [Required] string fileName,
            [FromQuery] GetAssetParamsDto dto)
        {
            var info = await _infoService.TryGetAssetInfoAsync(
                fileName, dto.Revision, dto.NeedHash);

            var result = info.BuildAdapter()
                    .AddParameters("baseUrl", _resourceService.BaseUrl)
                    .AdaptToType<AssetInfoDto>();

            return Ok(result);
        }
    }
}