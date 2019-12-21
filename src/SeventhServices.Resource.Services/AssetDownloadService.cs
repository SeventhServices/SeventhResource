using System;
using System.IO;
using System.Threading.Tasks;
using SeventhServices.Client.Network.Interfaces;
using SeventhServices.Resource.Common;
using SeventhServices.Resource.Common.Enums;
using SeventhServices.Resource.Common.Extensions;
using WebApiClient;

namespace SeventhServices.Resource.Services
{
    public class AssetDownloadService
    {
        private readonly IAssetDownloadClient _assetDownload;
        private readonly StatusService _statusService;

        public AssetDownloadService(IAssetDownloadClient assetDownload, StatusService statusService)
        {
            _assetDownload = assetDownload;
            _statusService = statusService;
        }

        public async Task DownloadCard(int cardId, FileSizeVersion sizeVersion = FileSizeVersion.Large)
        {

            var fileName = FileNameConverter.ToLargeCardFile(cardId);
            var file = sizeVersion switch
            {
                FileSizeVersion.Large => await _assetDownload.LargeCard(fileName)
                    .Retry(3, i => TimeSpan.FromSeconds(i)).WhenCatch<HttpStatusFailureException>(),
                FileSizeVersion.Middle => throw new NotImplementedException(),
                FileSizeVersion.Small => throw new NotImplementedException(),
                _ => throw new NullReferenceException()
            };
            var fileSavePath = _statusService.PathOption.AssetPath.AssetDownloadTempPath.AppendPath(fileName);
            await file.SaveAsAsync(fileSavePath);
            File.Copy(fileSavePath,
                _statusService.PathOption.AssetPath.GameMirrorAssetPath.AppendPath(fileName), true);

        }
    }
}