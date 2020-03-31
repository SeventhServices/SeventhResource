using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Seventh.Core.Dto.Request.Resource;
using Seventh.Core.Dto.Response.Resource;
using Seventh.Core.Services;
using Seventh.Resource.Common.Entities;
using Seventh.Resource.Common.Helpers;
using Seventh.Resource.Services;

namespace Seventh.Resource.Api.Controllers
{
    [ApiController]
    [ResponseCache(Duration = 120)]
    [Route("[controller]")]
    public class AssetController : Controller
    {
        private readonly SevenResourceService _resourceService;
        private readonly SevenStatusService _statusService;
        private readonly AssetInfoService _infoService;
        private readonly DownloadService _downloadService;
        private readonly QueueDownloadService _queueDownloadService;

        public AssetController(SevenResourceService resourceService,
            SevenStatusService statusService,
            AssetInfoService infoService,
            DownloadService downloadService,
            QueueDownloadService queueDownloadService)
        {
            _resourceService = resourceService;
            _statusService = statusService;
            _infoService = infoService;
            _downloadService = downloadService;
            _queueDownloadService = queueDownloadService;
        }

        [ResponseCache(Duration = 120)]
        [HttpPost("download/basic", Name = nameof(TryDownloadBasic))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<DownloadAssetDto>> TryDownloadBasic()
        {
            var modify = await _statusService.TryGetBasicModifyAsync();
            if (modify == null)
            {
                return NotFound();
            }

            var downloadFiles = new List<DownloadAssetDto>();
            var fileUrls = modify.ModifyFiles;
            foreach (var url in fileUrls)
            {
                _queueDownloadService.Enqueue(new DownloadFileTask 
                { 
                    FileName = url,
                    IsBasicDownload = true,
                    OverWrite = true
                });
                downloadFiles.Add(new DownloadAssetDto
                {
                    CanFound = true,
                    DownloadCompleted = false,
                    DownloadFileName = url
                });
            }
            _queueDownloadService.DequeueAll();
            return Ok(downloadFiles);
        }

        [ResponseCache(Duration = 120)]
        [HttpPost("download/card/{cardId}", Name = nameof(TryDownloadLargeCard))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<DownloadAssetDto>> TryDownloadLargeCard(
             [FromServices] OneByOneDownloadService downloadService, int cardId)
        {

            var (result, info) =
                  await downloadService.TryDownloadLargeCard(cardId);

            if (!result)
            {
                return DownloadFail(FileNameConverter.ToLargeCardFile(cardId), 0);
            }

            var downloadFileDto = new DownloadAssetDto
            {
                FileInfo = info.BuildAdapter()
                    .AddParameters("baseUrl", _resourceService.BaseUrl)
                    .AdaptToType<AssetInfoDto>()
            };
            downloadFileDto.DownloadCompleted = true;
            downloadFileDto.CanFound = true;
            downloadFileDto.DownloadFileName = info.MirrorFileInfo.Name;
            return Ok(downloadFileDto);
        }

        [ResponseCache(Duration = 1800)]
        [HttpPost("download", Name = nameof(TryDownloadAssets))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<DownloadAssetDto>>>
            TryDownloadAssets([FromBody] IEnumerable<GetAssetDto> dtoList)
        {
            var downloadFiles = new List<DownloadAssetDto>();

            foreach (var dto in dtoList)
            {
                DownloadAssetDto downloadFileDto;

                var info = await _infoService.TryGetAssetInfoAsync(
                    dto.FileName, dto.Revision, dto.NeedHash);

                if (info.MirrorFileInfo.Size != 0
                    && info.SortedFileInfo.Size != 0)
                {
                    downloadFileDto = new DownloadAssetDto
                    {
                        FileInfo = info.BuildAdapter()
                            .AddParameters("baseUrl", _resourceService.BaseUrl)
                            .AdaptToType<AssetInfoDto>()
                    };
                    downloadFileDto.DownloadFileName = dto.FileName;
                    downloadFileDto.DownloadCompleted = true;
                    downloadFileDto.CanFound = true;
                    downloadFiles.Add(downloadFileDto);
                    continue;
                }

                var (result, pass) = await _downloadService
                    .TryUsePolicyAsync(dto.FileName, dto.NeedHash, 10_000_000);

                if (!result)
                {
                    downloadFiles.Add(DownloadFail(dto.FileName));
                    continue;
                }

                if (!pass)
                {
                    downloadFileDto = new DownloadAssetDto
                    {
                        FileInfo = info.BuildAdapter()
                    .AddParameters("baseUrl", _resourceService.BaseUrl)
                    .AdaptToType<AssetInfoDto>()
                    };
                    downloadFileDto.DownloadFileName = dto.FileName;
                    downloadFileDto.DownloadCompleted = false;
                    downloadFileDto.CanFound = true;
                    downloadFiles.Add(downloadFileDto);
                    _queueDownloadService.Enqueue(dto.Adapt<DownloadFileTask>());
                    _queueDownloadService.DequeueAll();
                    continue;
                }

                if (dto.Revision != null)
                {
                    (result, info) =
                        await _downloadService.TryDownloadAtRevisionAndSortAsync(
                            dto.FileName, (int)dto.Revision, dto.NeedHash);
                }
                else
                {
                    (result, info) =
                        await _downloadService.TryDownloadAtMirrorAndSortAsync(
                            dto.FileName, dto.NeedHash);
                }

                if (!result)
                {
                    downloadFiles.Add(DownloadFail(dto.FileName));
                    continue;
                }

                downloadFileDto = new DownloadAssetDto
                {
                    FileInfo = info.BuildAdapter()
                    .AddParameters("baseUrl", _resourceService.BaseUrl)
                    .AdaptToType<AssetInfoDto>()
                };
                downloadFileDto.DownloadFileName = dto.FileName;
                downloadFileDto.DownloadCompleted = true;
                downloadFileDto.CanFound = true;
                downloadFiles.Add(downloadFileDto);
            }
            return Ok(downloadFiles);
        }

        [ResponseCache(Duration = 120)]
        [HttpPost("download/{fileName}", Name = nameof(TryGetDownloadAsset))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<DownloadAssetDto>> TryGetDownloadAsset(
            [RegularExpression("^.*\\..*$")] [Required] string fileName,
            [FromBody] GetAssetParamsDto dto)
        {
            DownloadAssetDto downloadFileDto;
            var info = await _infoService.TryGetAssetInfoAsync(
                fileName, dto.Revision, dto.NeedHash);

            if (info.MirrorFileInfo.Size != 0
                && info.SortedFileInfo.Size != 0)
            {
                downloadFileDto = new DownloadAssetDto
                {
                    FileInfo = info.BuildAdapter()
                    .AddParameters("baseUrl", _resourceService.BaseUrl)
                    .AdaptToType<AssetInfoDto>()
                };
                downloadFileDto.DownloadCompleted = true;
                return Ok(downloadFileDto);
            }

            var (result, pass) = await _downloadService
                .TryUsePolicyAsync(fileName, dto.NeedHash, 10_000_000);

            if (!result)
            {
                return NotFound(DownloadFail(fileName, dto.Revision));
            }

            if (!pass)
            {
                downloadFileDto = new DownloadAssetDto
                {
                    FileInfo = info.BuildAdapter()
                    .AddParameters("baseUrl", _resourceService.BaseUrl)
                    .AdaptToType<AssetInfoDto>()
                };
                downloadFileDto.DownloadCompleted = false;
                _queueDownloadService.Enqueue(dto.Adapt<DownloadFileTask>());
                _queueDownloadService.DequeueAll();
                return Ok(downloadFileDto);
            }

            if (dto.Revision != null)
            {
                (result, info) =
                    await _downloadService.TryDownloadAtRevisionAndSortAsync(
                        fileName, (int)dto.Revision, dto.NeedHash == true);
            }
            else
            {
                (result, info) =
                    await _downloadService.TryDownloadAtMirrorAndSortAsync(
                        fileName, dto.NeedHash == true);
            }

            if (!result)
            {
                return NotFound(DownloadFail(fileName, dto.Revision));
            }

            downloadFileDto = new DownloadAssetDto
            {
                FileInfo = info.BuildAdapter()
                    .AddParameters("baseUrl", _resourceService.BaseUrl)
                    .AdaptToType<AssetInfoDto>()
            };
            downloadFileDto.DownloadFileName = fileName;
            downloadFileDto.DownloadCompleted = true;
            downloadFileDto.CanFound = true;
            return Ok(downloadFileDto);
        }

        private DownloadAssetDto DownloadFail(
            string fileName, int? revision = 0)
        {
            return new DownloadAssetDto
            {
                CanFound = false,
                DownloadCompleted = false,
                DownloadFileName = fileName
            };
        }
    }
}