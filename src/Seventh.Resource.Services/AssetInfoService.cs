using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Seventh.Resource.Common.Crypts;
using Seventh.Resource.Common.Entities;
using Seventh.Resource.Common.Extensions;
using Seventh.Resource.Common.Helpers;
using Seventh.Resource.Common.Options;

namespace Seventh.Resource.Services
{
    public class AssetInfoService
    {
        private readonly SortService _sortService;
        private readonly PathOption _pathOption;

        public AssetInfoService(SortService sortService, ResourceLocation location)
        {
            _sortService = sortService;
            _pathOption = location.PathOption;
        }

        public async Task<ICollection<AssetFileInfo>> 
            TryGetFileInfoByRevAsync(int revision)
        {
            var directory = _pathOption.AssetPath
                .RevMirrorAssetPath.AppendPath(revision.ToString());
            if (!Directory.Exists(directory))
            {
                return null;
            }
            var infos = new List<AssetFileInfo>();
            var fileInfos = new DirectoryInfo(directory).GetFiles();
            long realFileSize = 0;
            foreach (var fileInfo in fileInfos)
            {
                var fileName = fileInfo.Name;
                var encVersion = AssetCrypt.IdentifyEncVersion(fileName);
                var realFileName = AssetCryptHelper.Rename(fileName, encVersion);
                var sortedSavePath = await _sortService.SortAsync(realFileName);

                if (File.Exists(sortedSavePath))
                {
                    realFileSize = new FileInfo(sortedSavePath).Length;
                }

                infos.Add(new AssetFileInfo
                {
                    FileName = fileName,
                    RealFileName = realFileName,
                    Revision = revision,
                    FileSize = fileInfo.Length,
                    RealFileSize = realFileSize,
                    MirrorSavePath = fileInfo.FullName.Replace(_pathOption.RootPath, string.Empty),
                    SortedSavePath = sortedSavePath?.Replace(_pathOption.RootPath, string.Empty)
                });
            }
            return infos;
        }

        public async Task<AssetFileInfo> 
            TryGetFileInfoAsync(string fileName, 
                int? revision = 0, bool needHash = false)
        {
            if (needHash)
            {
                fileName = FileNameConverter.ToWithHashName(fileName);
            }

            var isGameMirror = !revision.HasValue;
            long fileSize = 0; long realFileSize = 0;

            var savePath = isGameMirror 
                ? _pathOption.AssetPath.GameMirrorAssetPath.AppendPath(fileName)
                : _pathOption.AssetPath.RevMirrorAssetPath.AppendPath(revision.Value.ToString(),fileName);

            if (File.Exists(savePath))
            {
                fileSize = new FileInfo(savePath).Length;
            }

            var encVersion = AssetCrypt.IdentifyEncVersion(fileName);
            var realFileName = AssetCryptHelper.Rename(fileName, encVersion);
            var sortedSavePath = await _sortService.SortAsync(realFileName);

            if (File.Exists(sortedSavePath))
            {
                realFileSize = new FileInfo(sortedSavePath).Length;
            }

            return new AssetFileInfo
            {
                FileName = fileName,
                RealFileName = realFileName,
                Revision = isGameMirror ? 0 : revision.Value,
                FileSize = fileSize,
                RealFileSize = realFileSize,
                MirrorSavePath = savePath.Replace(_pathOption.RootPath, string.Empty),
                SortedSavePath = sortedSavePath.Replace(_pathOption.RootPath, string.Empty)
            };
        }
    }
}