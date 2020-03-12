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
    [Route("[controller]")]
    public class FileController : Controller
    {
        private readonly SevenResourceService _resourceService;
        private readonly AssetInfoService _infoService;
        private readonly DownloadService _downloadService;
        private readonly QueueDownloadService _queueDownloadService;

        public FileController(SevenResourceService resourceService,
            AssetInfoService infoService,
            DownloadService downloadService,
            QueueDownloadService queueDownloadService)
        {
            _resourceService = resourceService;
            _infoService = infoService;
            _downloadService = downloadService;
            _queueDownloadService = queueDownloadService;
        }
        [HttpPost("Download/Card/{CardId}")]
        [ResponseCache(Duration = 120)]
        public async Task<ActionResult<DownloadFileDto>> TryGetDownloadLargeCard(
    [FromServices] OneByOneDownloadService downloadService, int cardId)
        {

            var (result, info) =
                  await downloadService.TryDownloadLargeCard(cardId);

            if (!result)
            {
                return DownloadFail(FileNameConverter.ToLargeCardFile(cardId), 0);
            }

            var downloadFileDto =
                info.BuildAdapter()
                    .AddParameters("baseUrl", _resourceService.BaseUrl)
                    .AdaptToType<DownloadFileDto>();
            downloadFileDto.DownloadCompleted = true;
            downloadFileDto.CanFound = true;

            return Ok(downloadFileDto);
        }

        [HttpPost("Download")]
        [ResponseCache(Duration = 1800)]
        public async Task<ActionResult<IEnumerable<DownloadFileDto>>>
            TryDownloadFiles([FromBody] IEnumerable<GetFileDto> dtoList)
        {
            var downloadFiles = new List<DownloadFileDto>();

            foreach (var dto in dtoList)
            {
                DownloadFileDto downloadFileDto;

                var info = await _infoService.TryGetFileInfoAsync(
                    dto.FileName, dto.Revision, dto.NeedHash);

                if (info.FileSize != 0 && info.RealFileSize != 0)
                {
                    downloadFileDto =
                        info.BuildAdapter()
                            .AddParameters("baseUrl", _resourceService.BaseUrl)
                            .AdaptToType<DownloadFileDto>();
                    downloadFileDto.DownloadCompleted = true;
                    downloadFileDto.CanFound = true;
                    downloadFiles.Add(downloadFileDto);
                    continue;
                }

                var (result, pass) = await _downloadService
                    .TryUsePolicyAsync(dto.FileName, dto.NeedHash, 10_000_000);

                if (!result)
                {
                    downloadFiles.Add(new DownloadFileDto
                    {
                        CanFound = false,
                        DownloadCompleted = false,
                        FileName = dto.FileName,
                        Revision = dto.Revision ?? 0
                    });
                    continue;
                }

                if (!pass)
                {
                    downloadFileDto =
                        info.BuildAdapter()
                            .AddParameters("baseUrl", _resourceService.BaseUrl)
                            .AdaptToType<DownloadFileDto>();
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
                    downloadFiles.Add(new DownloadFileDto
                    {
                        CanFound = false,
                        DownloadCompleted = false,
                        FileName = dto.FileName,
                        Revision = dto.Revision ?? 0
                    });
                    continue;
                }

                downloadFileDto =
                    info.BuildAdapter()
                        .AddParameters("baseUrl", _resourceService.BaseUrl)
                        .AdaptToType<DownloadFileDto>();
                downloadFileDto.DownloadCompleted = true;
                downloadFileDto.CanFound = true;
                downloadFiles.Add(downloadFileDto);
            }
            return Ok(downloadFiles);
        }

        [HttpPost("Download/{FileName}")]
        [ResponseCache(Duration = 120)]
        public async Task<ActionResult<DownloadFileDto>> TryGetDownloadFile(
            [RegularExpression("^.*\\..*$")] [Required] string fileName,
            [FromBody] GetFileDtoParams dto)
        {
            DownloadFileDto downloadFileDto;
            var info = await _infoService.TryGetFileInfoAsync(
                fileName, dto.Revision, dto.NeedHash);

            if (info.FileSize != 0 && info.RealFileSize != 0)
            {
                downloadFileDto =
                    info.BuildAdapter()
                        .AddParameters("baseUrl", _resourceService.BaseUrl)
                        .AdaptToType<DownloadFileDto>();
                downloadFileDto.DownloadCompleted = true;
                return Ok(downloadFileDto);
            }

            var (result, pass) = await _downloadService
                .TryUsePolicyAsync(fileName, dto.NeedHash, 10_000_000);

            if (!result)
            {
                return DownloadFail(fileName, dto.Revision);
            }

            if (!pass)
            {
                downloadFileDto =
                    info.BuildAdapter()
                        .AddParameters("baseUrl", _resourceService.BaseUrl)
                        .AdaptToType<DownloadFileDto>();
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
                return DownloadFail(fileName, dto.Revision);
            }

            downloadFileDto =
                info.BuildAdapter()
                    .AddParameters("baseUrl", _resourceService.BaseUrl)
                    .AdaptToType<DownloadFileDto>();
            downloadFileDto.DownloadCompleted = true;
            downloadFileDto.CanFound = true;
            return Ok(downloadFileDto);
        }

        private ActionResult<DownloadFileDto> DownloadFail(
            string fileName, int? revision)
        {
            return NotFound(new DownloadFileDto
            {
                CanFound = false,
                DownloadCompleted = false,
                FileName = fileName,
                Revision = revision ?? 0
            });
        }
    }
}