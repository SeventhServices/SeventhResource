using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Extensions.Logging;
using Seventh.Resource.Common.Entities;
using Seventh.Resource.Common.Extensions;
using Seventh.Resource.Services.Abstractions;

namespace Seventh.Resource.Services
{
    public class BasicDownloadService : BaseDownloadService
    {
        private readonly HttpClient _client;

        public BasicDownloadService(OneByOneDownloadClient client, SortService sortService, AssetInfoProvider infoProvider, ResourceLocation location)
        : base(sortService, infoProvider, location)
        {
            _client = client.Client;
        }

        /// <summary>
        /// Try download and sort basic zip asset at BasicMirrorAssetPath in path option
        /// </summary>
        /// <param name="url">The downlaod url to basic zips </param>
        /// <param name="overWrite">wether</param>
        /// <returns>Download result and assetinfo</returns>
        public async Task<(bool result, AssetInfo info)>
            TryDownloadAndSortBasicZipAssetAsync(string url, bool overWrite)
        {
            var fileName = string.Join('_', url.Split('/').TakeLast(2));

            var savePath = LocalPathOption.AssetPath.BasicMirrorAssetPath.AppendPath(fileName);
            if (!File.Exists(savePath) || overWrite)
            {
                try
                {
                    var response = await _client.GetAsync(url);
                    savePath = await SaveFileAsync(fileName, savePath, response);
                    var fileList = ExtractZip(savePath);
                    foreach (var filePath in fileList)
                    {
                        await DecryptAndSortAsync(Path.GetFileName(filePath), filePath);
                    };
                }
                catch (HttpRequestException e)
                {
                    Logger?.LogError(e.ToString());
                    return (false, null);
                }
            }

            return (true, new AssetInfo().InitialUnsortedAsset(
                new AssetFileInfo
                {
                    Name = fileName,
                    Path = savePath,
                    Size = new FileInfo(savePath).Length
                }));
        }

        /// <summary>
        /// Extract zip to GameMirrorAssetPath
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IEnumerable<string> ExtractZip(string path)
        {
            var directoryPath = LocalPathOption.AssetPath.GameMirrorAssetPath;
            var extractFileList = new List<string>();
            var fastZipEvents = new FastZipEvents
            {
                CompletedFile = (s, e) =>
                {
                    Console.WriteLine($"Success extract {e.Name}");
                    var filePath = directoryPath.AppendPath(e.Name);
                    extractFileList.Add(filePath);
                }
            };
            new FastZip(fastZipEvents).ExtractZip(path, directoryPath, null);
            return extractFileList;
        }
    }
}
