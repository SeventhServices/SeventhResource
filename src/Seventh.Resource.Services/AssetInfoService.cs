using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly AssetInfoProvider _infoProvider;
        private readonly PathOption _pathOption;

        public AssetInfoService(SortService sortService, 
            AssetInfoProvider infoProvider,
            ResourceLocation location)
        {
            _sortService = sortService;
            _infoProvider = infoProvider;
            _pathOption = location.PathOption;
        }

        public Task<ICollection<string>>
            TryGetAllClassNamesAsync()
        {
            var directory = _pathOption.AssetPath
                .SortedAssetPath;
            if (!Directory.Exists(directory))
            {
                return null;
            }

            var classNameList = new List<string>();
            AddAllClassName(classNameList, new DirectoryInfo(directory));
            return Task.FromResult(classNameList as ICollection<string>);
        }

        public Task<ICollection<AssetFileInfo>>
            TryGetFileInfoByClassAsync(string className)
        {
            if (className == null) throw new ArgumentNullException(nameof(className));

            var directory = _pathOption.AssetPath
                .SortedAssetPath.AppendPath(className);

            if (!Directory.Exists(directory))
            {
                return Task.FromResult<ICollection<AssetFileInfo>>(null);
            }

            var fileInfos = new DirectoryInfo(directory).GetFiles();
            var infos = fileInfos.Select(fileInfo => new AssetFileInfo
                {
                    Name = fileInfo.Name,
                    Revision = FileNameHelper.ParseRev(fileInfo.Name),
                    Size = fileInfo.Length,
                    Path = fileInfo.FullName.Replace(_pathOption.RootPath, string.Empty)
                        .TrimStart(Path.DirectorySeparatorChar)
                }).ToArray();
            return Task.FromResult((ICollection<AssetFileInfo>) infos);
        }

        public async Task<ICollection<AssetInfo>>
            TryGetAssetInfoByRevAsync(int revision)
        {
            var directory = _pathOption.AssetPath
                .RevMirrorAssetPath.AppendPath(revision.ToString());
            if (!Directory.Exists(directory))
            {
                return null;
            }
            var infos = new List<AssetInfo>();
            var fileInfos = new DirectoryInfo(directory).GetFiles();
            long realFileSize = 0;
            foreach (var fileInfo in fileInfos)
            {
                var fileName = fileInfo.Name;
                var encVersion = AssetCrypt.IdentifyEncVersion(fileName);
                var realFileName = AssetCryptHelper.Rename(fileName, encVersion);
                var sortedSavePath = await _sortService.SortAsync(realFileName, revision);

                if (File.Exists(sortedSavePath))
                {
                    realFileSize = new FileInfo(sortedSavePath).Length;
                }

                var assetInfo = _infoProvider.ProvideAssetInfo(
                    fileName : fileName,
                    sortedFileName : realFileName,
                    size : fileInfo.Length,
                    sortedSize : realFileSize,
                    savePath : fileInfo.FullName.Replace(_pathOption.RootPath, string.Empty)
                        .TrimStart(Path.DirectorySeparatorChar),
                    sortedSavePath : sortedSavePath.Replace(_pathOption.RootPath, string.Empty)
                        .TrimStart(Path.DirectorySeparatorChar),
                    encrypted : encVersion != AssetCrypt.EncVersion.NoEnc
                );

                infos.Add(assetInfo.SetRevision(revision));
            }
            return infos;
        }

        public async Task<AssetInfo>
            TryGetAssetInfoAsync(string fileName,
                int? revision = 0, bool needHash = false)
        {
            if (needHash)
            {
                fileName = FileNameConverter.ToWithHashName(fileName);
            }

            var isGameMirror = !revision.HasValue;
            var savePath = isGameMirror
                ? _pathOption.AssetPath.GameMirrorAssetPath.AppendPath(fileName)
                : _pathOption.AssetPath.RevMirrorAssetPath.AppendPath(revision.Value.ToString(), fileName);

            var encVersion = AssetCrypt.IdentifyEncVersion(fileName);
            var realFileName = AssetCryptHelper.Rename(fileName, encVersion);
            var sortedSavePath = isGameMirror
                ? await _sortService.SortAsync(realFileName)
                : await _sortService.SortAsync(realFileName, revision.Value);

            var infoRevision = isGameMirror ? 0 : revision.Value;
            var assetInfo = _infoProvider.ProvideAssetInfo(
                fileName : fileName,
                sortedFileName : realFileName,
                savePath : savePath.Replace(_pathOption.RootPath, string.Empty)
                    .TrimStart(Path.DirectorySeparatorChar),
                sortedSavePath : sortedSavePath.Replace(_pathOption.RootPath, string.Empty)
                    .TrimStart(Path.DirectorySeparatorChar),
                encrypted : encVersion != AssetCrypt.EncVersion.NoEnc
            );
            return assetInfo.SetRevision(infoRevision);
        }

        private void AddAllClassName(ICollection<string> classNameList,
            DirectoryInfo baseDirectoryInfo)
        {
            var infos = baseDirectoryInfo.GetDirectories();
            while (infos.Length != 0)
            {
                foreach (var info in infos)
                {
                    classNameList.Add(info.FullName.Replace(_pathOption.AssetPath.SortedAssetPath, string.Empty)
                        .TrimStart(Path.DirectorySeparatorChar));
                    AddAllClassName(classNameList, info);
                }
                infos = Array.Empty<DirectoryInfo>();
            }
        }
    }
}