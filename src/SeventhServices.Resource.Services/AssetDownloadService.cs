using System;
using System.IO;
using System.Threading.Tasks;
using SeventhServices.Resource.Common;
using SeventhServices.Resource.Common.Enums;
using SeventhServices.Resource.Common.Extensions;

namespace SeventhServices.Resource.Services
{
    public class AssetDownloadService
    {
        private readonly OptionService _optionService;

        public AssetDownloadService(OptionService optionService)
        {
            _optionService = optionService;
        }

        //public async Task DownloadCard(int cardId, FileSizeVersion sizeVersion = FileSizeVersion.Large)
        //{

        //    var fileName = FileNameConverter.ToLargeCardFile(cardId);
        //    var file = sizeVersion switch
        //    {
        //        FileSizeVersion.Large => await _assetDownload.LargeCard(fileName)
        //            .Retry(3, i => TimeSpan.FromSeconds(i)).WhenCatch<HttpStatusFailureException>(),
        //        FileSizeVersion.Middle => throw new NotImplementedException(),
        //        FileSizeVersion.Small => throw new NotImplementedException(),
        //        _ => throw new NullReferenceException()
        //    };
        //    var fileSavePath = _optionService.PathOption.AssetPath.AssetDownloadTempPath.AppendPath(fileName);
        //    await file.SaveAsAsync(fileSavePath);
        //    File.Copy(fileSavePath,
        //        _optionService.PathOption.AssetPath.GameMirrorAssetPath.AppendPath(fileName), true);

        //}
    }
}