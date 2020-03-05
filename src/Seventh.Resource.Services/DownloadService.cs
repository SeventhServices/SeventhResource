using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Xml.Linq;
using Seventh.Resource.Common.Crypts;
using Seventh.Resource.Common.Entities;
using Seventh.Resource.Common.Extensions;
using Seventh.Resource.Common.Helpers;
using Seventh.Resource.Common.Options;

namespace Seventh.Resource.Services
{
    public class DownloadService
    {
        private readonly SortService _sortService;
        private readonly HttpClient _client;
        private readonly PathOption _pathOption;

        public DownloadService(DownloadClient client, SortService sortService, ResourceLocation location)
        {
            _sortService = sortService;
            _client = client.Client;
            _pathOption = location.PathOption;
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

            var info = await DecryptAndSort(fileName, string.Concat(_pathOption.RootPath, savePath), revision);
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

        public async Task<(bool result, long? Length)> 
            TryGetContentLengthAsync(string fileName)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(fileName, UriKind.RelativeOrAbsolute) ,
                Method = HttpMethod.Head
            };

            var response = await _client.SendAsync(request,HttpCompletionOption.ResponseHeadersRead)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                return (false, 0);
            }

            return (true, response.Content.Headers.ContentLength);
        }



        public async Task<(bool result ,bool pass)> TryUsePolicyAsync(string fileName, bool needHash, long maxLength, bool whenLengthNull = true)
        {
            if (needHash)
            {
                fileName = FileNameConverter.ToWithHashName(fileName);
            }

            var (result, length) = await TryGetContentLengthAsync(fileName);

            if (!result) return (false, false);

            return length == null ? (true, false) : (true,!(length > maxLength));
        }

        private async Task<string> SaveFile(string fileName, string savePath, HttpResponseMessage response)
        {
            var tempSavePath = _pathOption.AssetPath.TempRootPath.AppendPath(fileName);
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
            var realFileName = AssetCryptHelper.Rename(fileName, encVersion);
            var sortedSavePath = await _sortService.SortAsync(realFileName);

            if (File.Exists(sortedSavePath))
            {
                return new AssetFileInfo
                {
                    FileName = fileName,
                    RealFileName = realFileName,
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
                    FileName = fileName,
                    RealFileName = realFileName,
                    FileSize = new FileInfo(savePath).Length,
                    RealFileSize = new FileInfo(sortedSavePath).Length,
                    MirrorSavePath = savePath,
                    SortedSavePath = sortedSavePath
                };
            }

            await AssetCryptHelper.DecryptAsync(savePath, sortedSavePath, encVersion,
                AssetCryptHelper.IdentifyShouldLz4(realFileName));

            return new AssetFileInfo
            {
                FileName = fileName,
                RealFileName = realFileName,
                FileSize = new FileInfo(savePath).Length,
                RealFileSize = new FileInfo(sortedSavePath).Length,
                MirrorSavePath = savePath,
                SortedSavePath = sortedSavePath
            };
        }

        private async Task<AssetFileInfo>
            DecryptAndSort(string fileName, string savePath, int revision)
        {
            var encVersion = AssetCrypt.IdentifyEncVersion(fileName);
            var realFileName = AssetCryptHelper.Rename(fileName, encVersion);
            var sortedSavePath = await _sortService.SortAsync(realFileName, revision);

            if (File.Exists(sortedSavePath))
            {
                return new AssetFileInfo
                {
                    FileName = fileName,
                    RealFileName = realFileName,
                    FileSize = new FileInfo(savePath).Length,
                    RealFileSize = new FileInfo(sortedSavePath).Length,
                    MirrorSavePath = savePath,
                    SortedSavePath = sortedSavePath
                };
            }

            if (encVersion == AssetCrypt.EncVersion.NoEnc)
            {
                File.Copy(savePath, sortedSavePath,true);
                return new AssetFileInfo
                {
                    FileName = fileName,
                    RealFileName = realFileName,
                    FileSize = new FileInfo(savePath).Length,
                    RealFileSize = new FileInfo(sortedSavePath).Length,
                    MirrorSavePath = savePath,
                    SortedSavePath = sortedSavePath
                };
            }

            await AssetCryptHelper.DecryptAsync(savePath, sortedSavePath, encVersion,
                AssetCryptHelper.IdentifyShouldLz4(realFileName));

            return new AssetFileInfo
            {
                FileName = fileName,
                RealFileName = realFileName,
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