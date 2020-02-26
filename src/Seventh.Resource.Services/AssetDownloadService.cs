using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Seventh.Resource.Common.Classes.Options;
using Seventh.Resource.Common.Crypts;
using Seventh.Resource.Common.Extensions;
using Seventh.Resource.Common.Helpers;

namespace Seventh.Resource.Services
{
    public class AssetDownloadService
    {
        private readonly AssetSortService _sortService;
        private readonly HttpClient _client;
        private readonly AssetPath _assetPath;

        public AssetDownloadService(AssetDownloadClient client, AssetSortService sortService,ResourceLocation option)
        {
            _sortService = sortService;
            _client = client.Client;
            _assetPath = option.PathOption.AssetPath;
        }

        public async Task<(bool result, string savePath, string sortedSavePath)>
            TryDownloadAtRevisionAndSortAsync(string fileName, int revision, bool needHash)
        {
            if (needHash)
            {
                fileName = FileNameConverter.ToWithHashName(fileName);
            }

            var (result, savePath) = await TryDownloadAtRevisionAsync(fileName, revision,false);
            if (!result)
            {
                return (false, null, null);
            }

            return await DecryptAndSort(fileName, savePath);
        }


        public async Task<(bool result, string savePath, string sortedSavePath)>
            TryDownloadAtMirrorAndSortAsync(string fileName, bool needHash)
        {
            if (needHash)
            {
                fileName = FileNameConverter.ToWithHashName(fileName);
            }

            var (result,savePath) = await TryDownloadAtMirrorAsync(fileName, true);
            if (!result)
            {
                return (false,null,null);
            }

            return await DecryptAndSort(fileName, savePath);
        }

        public async Task<(bool result, string savePath)>
            TryDownloadAtRevisionAsync(string fileName, int revision ,bool needHash)
        {
            if (needHash)
            {
                fileName = FileNameConverter.ToWithHashName(fileName);
            }

            var savePath = _assetPath.RevMirrorAssetPath
                .AppendAndCreatePath(revision.ToString()).AppendPath(fileName);

            if (File.Exists(savePath))
            {
                return (true, savePath);
            }

            var response = await _client.GetAsync(fileName);
            return !response.IsSuccessStatusCode 
                ? (false, null) 
                : (true, await SaveFile(fileName, savePath, response));
        }

        public async Task<(bool result, string savePath)>
            TryDownloadAtMirrorAsync(string fileName, bool needHash)
        {
            if (needHash)
            {
                fileName = FileNameConverter.ToWithHashName(fileName);
            }

            var savePath = _assetPath.GameMirrorAssetPath.AppendPath(fileName);

            if (File.Exists(savePath))
            {
                return (true, savePath);
            }

            var response = await _client.GetAsync(fileName);
            return !response.IsSuccessStatusCode 
                ? (false, null) 
                : (true, await SaveFile(fileName, savePath, response));
        }

        private async Task<string> SaveFile(string fileName, string savePath,HttpResponseMessage response)
        {
            var tempSavePath = _assetPath.DownloadTempRootPath.AppendPath(fileName);
            await using var fileStream = File.OpenWrite(tempSavePath);
            await response.Content.CopyToAsync(fileStream);
            fileStream.Close();
            File.Copy(tempSavePath,
                savePath, true);
            File.Delete(tempSavePath);
            return savePath;
        }

        private async Task<(bool result, string savePath, string sortedSavePath)>
            DecryptAndSort(string fileName, string savePath)
        {
            var encVersion = AssetCrypt.IdentifyEncVersion(fileName);
            fileName = AssetCryptHelper.Rename(fileName, encVersion);
            var sortedSavePath = await _sortService.SortAsync(fileName);
            if (encVersion == AssetCrypt.EncVersion.NoEnc)
            {
                File.Copy(savePath, sortedSavePath);
                return (true, savePath, sortedSavePath);
            }

            await AssetCryptHelper.DecryptAsync(savePath, sortedSavePath, encVersion,
                AssetCryptHelper.IdentifyShouldLz4(fileName));
            return (true, savePath, sortedSavePath);
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