﻿using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Seventh.Resource.Common.Crypts;
using Seventh.Resource.Common.Entities;
using Seventh.Resource.Common.Extensions;
using Seventh.Resource.Common.Helpers;
using Seventh.Resource.Common.Options;

namespace Seventh.Resource.Services
{
    public class AssetDownloadService
    {
        private readonly AssetSortService _sortService;
        private readonly HttpClient _client;
        private readonly PathOption _pathOption;

        public AssetDownloadService(AssetDownloadClient client, AssetSortService sortService, ResourceLocation option)
        {
            _sortService = sortService;
            _client = client.Client;

            _pathOption = option.PathOption;
        }

        public async Task<(bool result, AssetFileInfo info)>
            TryDownloadAtRevisionAndSortAsync(string fileName, int revision, bool needHash)
        {
            if (needHash)
            {
                fileName = FileNameConverter.ToWithHashName(fileName);
            }

            var (result, savePath) = await TryDownloadAtRevisionAsync(fileName, revision, needHash);
            if (!result)
            {
                return (false, null);
            }

            var info = await DecryptAndSort(fileName, string.Concat(_pathOption.RootPath, savePath));
            info.Revision = revision;
            info.MirrorSavePath = info.MirrorSavePath.Replace(_pathOption.RootPath, string.Empty);
            info.SortedSavePath = info.SortedSavePath.Replace(_pathOption.RootPath, string.Empty);
            return (true, info);
        }

        public async Task<(bool result, AssetFileInfo info)>
            TryDownloadAtMirrorAndSortAsync(string fileName, bool needHash)
        {
            if (needHash)
            {
                fileName = FileNameConverter.ToWithHashName(fileName);
            }

            var (result, savePath) = await TryDownloadAtMirrorAsync(fileName, needHash);
            if (!result)
            {
                return (false, null);
            }
            
            var info = await DecryptAndSort(fileName, string.Concat(_pathOption.RootPath, savePath));
            info.Revision = 0;
            info.MirrorSavePath = info.MirrorSavePath.Replace(_pathOption.RootPath, string.Empty);
            info.SortedSavePath = info.SortedSavePath.Replace(_pathOption.RootPath, string.Empty);
            return (true,info);
        }

        public async Task<(bool result, string savePath)>
            TryDownloadAtRevisionAsync(string fileName, int revision, bool needHash)
        {
            if (needHash)
            {
                fileName = FileNameConverter.ToWithHashName(fileName);
            }

            var savePath = _pathOption.AssetPath.RevMirrorAssetPath
                .AppendAndCreatePath(revision.ToString()).AppendPath(fileName);

            if (File.Exists(savePath))
            {
                return (true, savePath.Replace(_pathOption.RootPath,string.Empty));
            }

            var response = await _client.GetAsync(fileName);
            return !response.IsSuccessStatusCode
                ? (false, null)
                : (true, (await SaveFile(fileName, savePath, response))
                    .Replace(_pathOption.RootPath,string.Empty));
        }

        public async Task<(bool result, string savePath)>
            TryDownloadAtMirrorAsync(string fileName, bool needHash)
        {
            if (needHash)
            {
                fileName = FileNameConverter.ToWithHashName(fileName);
            }

            var savePath = _pathOption.AssetPath.GameMirrorAssetPath.AppendPath(fileName);

            if (File.Exists(savePath))
            {
                return (true, savePath);
            }

            var response = await _client.GetAsync(fileName);
            return !response.IsSuccessStatusCode
                ? (false, null)
                : (true, (await SaveFile(fileName, savePath, response))
                    .Replace(_pathOption.RootPath,string.Empty));
        }

        private async Task<string> SaveFile(string fileName, string savePath, HttpResponseMessage response)
        {
            var tempSavePath = _pathOption.AssetPath.DownloadTempRootPath.AppendPath(fileName);
            await using var fileStream = File.OpenWrite(tempSavePath);
            await response.Content.CopyToAsync(fileStream);
            fileStream.Close();
            File.Copy(tempSavePath,
                savePath, true);
            File.Delete(tempSavePath);
            return savePath;
        }

        private async Task<AssetFileInfo>
            DecryptAndSort(string fileName, string savePath)
        {
            var encVersion = AssetCrypt.IdentifyEncVersion(fileName);
            var encFileName = fileName;
            fileName = AssetCryptHelper.Rename(fileName, encVersion);
            var sortedSavePath = await _sortService.SortAsync(fileName);

            if (File.Exists(sortedSavePath))
            {
                return new AssetFileInfo
                {
                    FileName = encFileName,
                    RealFileName = fileName,
                    FileSize = new FileInfo(savePath).Length,
                    RealFileSize = new FileInfo(sortedSavePath).Length,
                    MirrorSavePath = savePath,
                    SortedSavePath = sortedSavePath
                };
            }

            if (encVersion == AssetCrypt.EncVersion.NoEnc)
            {
                File.Copy(savePath, sortedSavePath);
                return new AssetFileInfo
                {
                    FileName = encFileName,
                    RealFileName = fileName,
                    FileSize = new FileInfo(savePath).Length,
                    RealFileSize = new FileInfo(sortedSavePath).Length,
                    MirrorSavePath = savePath,
                    SortedSavePath = sortedSavePath
                };
            }

            await AssetCryptHelper.DecryptAsync(savePath, sortedSavePath, encVersion,
                AssetCryptHelper.IdentifyShouldLz4(fileName));

            return new AssetFileInfo
            {
                FileName = encFileName,
                RealFileName = fileName,
                FileSize = new FileInfo(savePath).Length,
                RealFileSize = new FileInfo(sortedSavePath).Length,
                MirrorSavePath = savePath,
                SortedSavePath = sortedSavePath
            };
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