using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Seventh.Resource.Common.Entities;
using Seventh.Resource.Common.Extensions;
using Seventh.Resource.Common.Helpers;
using Seventh.Resource.Services.Abstractions;

namespace Seventh.Resource.Services
{
    public class DownloadService : BaseDownloadService
    {
        private readonly HttpClient _client;

        public DownloadService(DownloadClient client, SortService sortService, AssetInfoProvider infoProvider, ResourceLocation location)
            : base(sortService, infoProvider, location)
        {
            _client = client.Client;
        }

        public async Task<(bool result, AssetInfo info)>
            TryDownloadAtRevisionAndSortAsync(string fileName, int revision, bool needHash)
        {
            if (needHash)
            {
                fileName = FileNameConverter.ToWithHashName(fileName);
            }

            var (result, savePath) = await TryDownloadAtRevisionAsync(fileName, revision, false);
            if (!result)
            {
                return (false, null);
            }

            var info = await DecryptAndSortAsync(fileName, string.Concat(LocalPathOption.RootPath, savePath), revision);
            info.SetRevision(revision);
            info.Path = info.Path.Replace(LocalPathOption.RootPath, string.Empty);
            info.SortedPath = info.SortedPath.Replace(LocalPathOption.RootPath, string.Empty);
            return (true, info);
        }

        public async Task<(bool result, AssetInfo info)>
            TryDownloadAtMirrorAndSortAsync(string fileName, bool needHash)
        {
            if (needHash)
            {
                fileName = FileNameConverter.ToWithHashName(fileName);
            }

            var (result, savePath) = await TryDownloadAtMirrorAsync(fileName, false);
            if (!result)
            {
                return (false, null);
            }

            var info = await DecryptAndSortAsync(fileName, string.Concat(LocalPathOption.RootPath, savePath));
            info.SetRevision(0);
            info.Path = info.Path.Replace(LocalPathOption.RootPath, string.Empty);
            info.SortedPath = info.SortedPath.Replace(LocalPathOption.RootPath, string.Empty);
            return (true, info);

        }

        public async Task<(bool result, string savePath)>
            TryDownloadAtRevisionAsync(string fileName, int revision, bool needHash)
        {
            if (needHash)
            {
                fileName = FileNameConverter.ToWithHashName(fileName);
            }

            var savePath = LocalPathOption.AssetPath.RevMirrorAssetPath
                .AppendAndCreatePath(revision.ToString()).AppendPath(fileName);

            if (File.Exists(savePath))
            {
                return (true, savePath.Replace(LocalPathOption.RootPath, string.Empty));
            }

            var response = await _client.GetAsync(fileName);
            return !response.IsSuccessStatusCode
                ? (false, null)
                : (true, (await SaveFileAsync(fileName, savePath, response))
                    .Replace(LocalPathOption.RootPath, string.Empty));
        }

        public async Task<(bool result, string savePath)>
            TryDownloadAtMirrorAsync(string fileName, bool needHash)
        {
            if (needHash)
            {
                fileName = FileNameConverter.ToWithHashName(fileName);
            }

            var savePath = LocalPathOption.AssetPath.GameMirrorAssetPath.AppendPath(fileName);

            if (File.Exists(savePath))
            {
                return (true, savePath.Replace(LocalPathOption.RootPath, string.Empty));
            }

            var response = await _client.GetAsync(fileName);
            return !response.IsSuccessStatusCode
                ? (false, null)
                : (true, (await SaveFileAsync(fileName, savePath, response))
                    .Replace(LocalPathOption.RootPath, string.Empty));
        }

        public async Task<(bool result, long? Length)>
            TryGetContentLengthAsync(string fileName)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(fileName, UriKind.RelativeOrAbsolute),
                Method = HttpMethod.Head
            };

            var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                return (false, 0);
            }

            return (true, response.Content.Headers.ContentLength);
        }

        public async Task<(bool result, bool pass)> TryUsePolicyAsync(string fileName, bool needHash, long maxLength, bool whenLengthNull = true)
        {
            if (needHash)
            {
                fileName = FileNameConverter.ToWithHashName(fileName);
            }

            var (result, length) = await TryGetContentLengthAsync(fileName);

            if (!result) return (false, false);

            return length == null ? (true, whenLengthNull) : (true, !(length > maxLength));
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