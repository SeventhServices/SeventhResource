using System.IO;
using Seventh.Resource.Common.Options;
using System.Net.Http;
using System.Threading.Tasks;
using Seventh.Resource.Common.Crypts;
using Seventh.Resource.Common.Entities;
using Seventh.Resource.Common.Extensions;
using Seventh.Resource.Common.Helpers;
using Seventh.Resource.Services.Abstractions;
using System.Runtime.CompilerServices;

namespace Seventh.Resource.Services
{
    public class OneByOneDownloadService : BaseDownloadService
    {
        private readonly HttpClient _client;

        public OneByOneDownloadService(OneByOneDownloadClient client, SortService sortService,ResourceLocation location)
        : base(sortService,location)
        {
            _client = client.Client;
        }

        public async Task<(bool result, AssetInfo info)>
            TryDownloadLargeCard(int cardId)
        {
            var fileName = FileNameConverter.ToLargeCardFile(cardId);
            var url = string.Concat("resource/images/card/l/",fileName);

            return await TryDownloadAtMirrorAndSortAsync(url,fileName);
        }

        public async Task<(bool result, AssetInfo info)>
            TryDownloadAtMirrorAndSortAsync(string url, string fileName)
        {
            var (result, savePath) = await TryDownloadAtMirrorAsync(url,fileName);
            if (!result)
            {
                return (false, null);
            }
            
            var info = await DecryptAndSort(fileName, string.Concat(LocalPathOption.RootPath, savePath));
            info.SetRevision(0);
            info.MirrorFileInfo.Path = info.MirrorFileInfo.Path.Replace(LocalPathOption.RootPath, string.Empty);
            info.SortedFileInfo.Path = info.SortedFileInfo.Path.Replace(LocalPathOption.RootPath, string.Empty);
            return (true,info);
        }


        public async Task<(bool result, string savePath)>
            TryDownloadAtMirrorAsync(string url, string fileName)
        {
            var savePath = LocalPathOption.AssetPath.GameMirrorAssetPath.AppendPath(fileName);

            if (File.Exists(savePath))
            {
                return (true, savePath.Replace(LocalPathOption.RootPath,string.Empty));
            }

            var response = await _client.GetAsync(url);
            return !response.IsSuccessStatusCode
                ? (false, null)
                : (true, (await SaveFile(fileName, savePath, response))
                    .Replace(LocalPathOption.RootPath,string.Empty));
        }

    }
}