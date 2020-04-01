using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using Seventh.Resource.Common.Entities;
using Seventh.Resource.Common.Extensions;
using Seventh.Resource.Services.Abstractions;

namespace Seventh.Resource.Services
{
    public class BasicDownloadService : BaseDownloadService
    {
        private readonly HttpClient _client;

        public BasicDownloadService(OneByOneDownloadClient client, SortService sortService, ResourceLocation location)
        : base(sortService, location)
        {
            _client = client.Client;
        }

        public async Task<(bool result, AssetInfo info)>
            TryDownloadAndSortBasicZipAssetAsync(string url, bool overWrite)
        {
            var fileName = string.Join('_', url.Split('/').TakeLast(2));

            var savePath = LocalPathOption.AssetPath.BasicMirrorAssetPath.AppendPath(fileName);
            if (!File.Exists(savePath) || overWrite)
            {
                var response = await _client.GetAsync(url);
                if (response is null)
                {
                    return (false, null);
                }
                savePath = await SaveFileAsync(fileName, savePath, response);
                var fileList = ExtractZip(savePath);

                foreach (var filePath in fileList)
                {
                    try
                    {
                        await DecryptAndSortAsync(Path.GetFileName(filePath), filePath);
                    }
                    catch (CryptographicException e)
                    {
                        Console.WriteLine($"{filePath}:{e}");
                    }
                };
            }

            return (true, new AssetInfo
            {
                MirrorFileInfo = new AssetFileInfo
                {
                    Name = fileName,
                    Path = savePath,
                    Size = new FileInfo(savePath).Length
                }
            });
        }

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
