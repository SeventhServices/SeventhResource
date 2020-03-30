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
        private readonly PathOption _pathOption;

        public AssetInfoService(SortService sortService, ResourceLocation location)
        {
            _sortService = sortService;
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

            var infos = new List<AssetFileInfo>();
            var fileInfos = new DirectoryInfo(directory).GetFiles();
            foreach (var fileInfo in fileInfos)
            {
                infos.Add(new AssetFileInfo
                {
                    Name = fileInfo.Name,
                    Revision = ParseRev(fileInfo.Name),
                    Size = fileInfo.Length,
                    Path = fileInfo.FullName.Replace(_pathOption.RootPath, string.Empty)
                });
            }
            return Task.FromResult(infos as ICollection<AssetFileInfo>);
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

                infos.Add(new AssetInfo
                {
                    MirrorFileInfo = new AssetFileInfo
                    {
                        Name = fileName,
                        Revision = revision,
                        Size = fileInfo.Length,
                        Path = fileInfo.FullName.Replace(_pathOption.RootPath, string.Empty)
                    },
                    SortedFileInfo = new AssetFileInfo
                    {
                        Name = realFileName,
                        Revision = revision,
                        Size = realFileSize,
                        Path = sortedSavePath?.Replace(_pathOption.RootPath, string.Empty)
                    }
                });
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
            long fileSize = 0; long realFileSize = 0;

            var savePath = isGameMirror
                ? _pathOption.AssetPath.GameMirrorAssetPath.AppendPath(fileName)
                : _pathOption.AssetPath.RevMirrorAssetPath.AppendPath(revision.Value.ToString(), fileName);

            if (File.Exists(savePath))
            {
                fileSize = new FileInfo(savePath).Length;
            }

            var encVersion = AssetCrypt.IdentifyEncVersion(fileName);
            var realFileName = AssetCryptHelper.Rename(fileName, encVersion);
            var sortedSavePath = isGameMirror
                ? await _sortService.SortAsync(realFileName)
                : await _sortService.SortAsync(realFileName, revision.Value);

            if (File.Exists(sortedSavePath))
            {
                realFileSize = new FileInfo(sortedSavePath).Length;
            }

            var infoRevision = isGameMirror ? 0 : revision.Value;
            return new AssetInfo
            {
                MirrorFileInfo = new AssetFileInfo
                {
                    Name = fileName,
                    Revision = infoRevision,
                    Size = fileSize,
                    Path = savePath.Replace(_pathOption.RootPath, string.Empty),
                },
                SortedFileInfo = new AssetFileInfo
                {
                    Name = realFileName,
                    Revision = infoRevision,
                    Size = realFileSize,
                    Path = sortedSavePath?.Replace(_pathOption.RootPath, string.Empty)
                }
            };
        }

        private int ParseRev(string fileName)
        {
            var revNamePart = Path.GetFileNameWithoutExtension(fileName).Split('_').LastOrDefault();
            if (revNamePart == null)
            {
                return 0;
            }

            if (revNamePart.StartsWith('r'))
            {
                revNamePart = revNamePart.TrimStart('r');
                return int.TryParse(revNamePart, out int revision) ? revision : 0;
            }

            return 0;
        }

        private void AddAllClassName(ICollection<string> classNameList,
            DirectoryInfo baseDirectoryInfo)
        {
            var infos = baseDirectoryInfo.GetDirectories();
            while (infos.Length != 0)
            {
                foreach (var info in infos)
                {
                    classNameList.Add(info.FullName.Replace(string.Concat(_pathOption.AssetPath.SortedAssetPath, Path.DirectorySeparatorChar), string.Empty));
                    AddAllClassName(classNameList, info);
                }
                infos = Array.Empty<DirectoryInfo>();
            }
        }
    }
}