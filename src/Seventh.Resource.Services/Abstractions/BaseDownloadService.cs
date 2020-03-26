using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Seventh.Resource.Common.Crypts;
using Seventh.Resource.Common.Entities;
using Seventh.Resource.Common.Extensions;
using Seventh.Resource.Common.Helpers;
using Seventh.Resource.Common.Options;

namespace Seventh.Resource.Services.Abstractions
{
    public abstract class BaseDownloadService
    {
        private readonly SortService _sortService;
        protected readonly PathOption LocalPathOption;

        protected BaseDownloadService(SortService sortService, ResourceLocation location)
        {
            _sortService = sortService;
            LocalPathOption = location.PathOption;
        }

        protected async Task<string> SaveFile(string fileName, string savePath, HttpResponseMessage response)
        {
            var tempSavePath = LocalPathOption.AssetPath.TempRootPath.AppendPath(fileName);
            await using var fileStream = File.OpenWrite(tempSavePath);
            await response.Content.CopyToAsync(fileStream);
            fileStream.Close();
            File.Copy(tempSavePath,
                savePath, true);
            File.Delete(tempSavePath);
            return savePath;
        }

        protected async Task<AssetInfo>
            DecryptAndSort(string fileName, string savePath)
        {
            var encVersion = AssetCrypt.IdentifyEncVersion(fileName);
            var realFileName = AssetCryptHelper.Rename(fileName, encVersion);
            var sortedSavePath = await _sortService.SortAsync(realFileName);

            if (File.Exists(sortedSavePath))
            {
                return new AssetInfo
                {
                    MirrorFileInfo = new AssetFileInfo
                    {
                        Name = fileName,
                        Size = new FileInfo(savePath).Length,
                        Path = savePath
                    },
                    SortedFileInfo = new AssetFileInfo
                    {
                        Name = realFileName,
                        Size = new FileInfo(sortedSavePath).Length,
                        Path = sortedSavePath
                    }
                };
            }

            if (encVersion == AssetCrypt.EncVersion.NoEnc)
            {
                File.Copy(savePath, sortedSavePath);
                return new AssetInfo
                {
                    MirrorFileInfo = new AssetFileInfo
                    {
                        Name = fileName,
                        Size = new FileInfo(savePath).Length,
                        Path = savePath
                    },
                    SortedFileInfo = new AssetFileInfo
                    {
                        Name = realFileName,
                        Size = new FileInfo(sortedSavePath).Length,
                        Path = sortedSavePath
                    }
                };
            }

            await AssetCryptHelper.DecryptAsync(savePath, sortedSavePath, encVersion,
                AssetCryptHelper.IdentifyShouldLz4(realFileName));
            return new AssetInfo
            {
                MirrorFileInfo = new AssetFileInfo
                {
                    Name = fileName,
                    Size = new FileInfo(savePath).Length,
                    Path = savePath
                },
                SortedFileInfo = new AssetFileInfo
                {
                    Name = realFileName,
                    Size = new FileInfo(sortedSavePath).Length,
                    Path = sortedSavePath
                }
            };
        }

        protected async Task<AssetInfo>
            DecryptAndSort(string fileName, string savePath, int revision)
        {
            var encVersion = AssetCrypt.IdentifyEncVersion(fileName);
            var realFileName = AssetCryptHelper.Rename(fileName, encVersion);
            var sortedSavePath = await _sortService.SortAsync(realFileName);
            var revSortedSavePath = await _sortService.SortAsync(realFileName, revision);

            if (File.Exists(revSortedSavePath))
            {
                return new AssetInfo
                {
                    MirrorFileInfo = new AssetFileInfo
                    {
                        Name = fileName,
                        Size = new FileInfo(savePath).Length,
                        Path = savePath
                    },
                    SortedFileInfo = new AssetFileInfo
                    {
                        Name = realFileName,
                        Size = new FileInfo(revSortedSavePath).Length,
                        Path = revSortedSavePath
                    }
                };
            }

            if (encVersion == AssetCrypt.EncVersion.NoEnc)
            {
                File.Copy(savePath, sortedSavePath, true);
                return new AssetInfo
                {
                    MirrorFileInfo = new AssetFileInfo
                    {
                        Name = fileName,
                        Size = new FileInfo(savePath).Length,
                        Path = savePath
                    },
                    SortedFileInfo = new AssetFileInfo
                    {
                        Name = realFileName,
                        Size = new FileInfo(sortedSavePath).Length,
                        Path = sortedSavePath
                    }
                };
            }

            await AssetCryptHelper.DecryptAsync(savePath, sortedSavePath, encVersion,
                AssetCryptHelper.IdentifyShouldLz4(realFileName));

            if (!revSortedSavePath.Equals(sortedSavePath))
            {
                File.Copy(sortedSavePath, revSortedSavePath, true);
            }

            return new AssetInfo
            {
                MirrorFileInfo = new AssetFileInfo
                {
                    Name = fileName,
                    Size = new FileInfo(savePath).Length,
                    Path = savePath
                },
                SortedFileInfo = new AssetFileInfo
                {
                    Name = realFileName,
                    Size = new FileInfo(revSortedSavePath).Length,
                    Path = revSortedSavePath
                }
            };
        }
    }
}