using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        private readonly AssetInfoProvider _infoProvider;
        protected readonly PathOption LocalPathOption;

        protected BaseDownloadService(SortService sortService,
            AssetInfoProvider infoProvider,
            ResourceLocation location)
        {
            _sortService = sortService;
            _infoProvider = infoProvider;
            LocalPathOption = location.PathOption;
        }

        public ILogger<BaseDownloadService> Logger { get; set; }

        protected async Task<string> SaveFileAsync(string fileName, string savePath, HttpResponseMessage response)
        {
            if (response is null)
            {
                throw new System.ArgumentNullException(nameof(response));
            }

            var tempSavePath = LocalPathOption.AssetPath.AssetTempPath.AppendPath(fileName);
            await using var fileStream = File.OpenWrite(tempSavePath);
            await response.Content.CopyToAsync(fileStream);
            fileStream.Close();
            File.Copy(tempSavePath,
                savePath, true);
            File.Delete(tempSavePath);
            return savePath;
        }

        protected async Task<AssetInfo>
            DecryptAndSortAsync(string fileName, string savePath)
        {
            var encVersion = AssetCrypt.IdentifyEncVersion(fileName);
            var realFileName = AssetCryptHelper.Rename(fileName, encVersion);
            var sortedSavePath = await _sortService.SortAsync(realFileName);

            var encrypted = encVersion != AssetCrypt.EncVersion.NoEnc;

            if (File.Exists(sortedSavePath))
            {
                return _infoProvider.ProvideAssetInfo(fileName, realFileName, savePath, sortedSavePath, encrypted);
            }

            if (!encrypted)
            {
                File.Copy(savePath, sortedSavePath);
                return _infoProvider.ProvideAssetInfo(fileName, realFileName, savePath, sortedSavePath, false);
            }

            try
            {
                await AssetCryptHelper.DecryptAsync(savePath, sortedSavePath, encVersion,
                    AssetCryptHelper.IdentifyShouldLz4(realFileName));
            }
            catch (Exception e)
            {
                // if the enc file can not be decrypt,
                // then directly copy it to the sorted directory.
                Logger?.LogError(e.ToString());
                realFileName = fileName;
                sortedSavePath = await _sortService.SortAsync(realFileName);
                File.Copy(savePath, sortedSavePath, true);
            }

            return _infoProvider.ProvideAssetInfo(fileName, realFileName, savePath, sortedSavePath, true);
        }

        protected async Task<AssetInfo>
            DecryptAndSortAsync(string fileName, string savePath, int revision, bool overWrite = false)
        {
            var encVersion = AssetCrypt.IdentifyEncVersion(fileName);
            var realFileName = AssetCryptHelper.Rename(fileName, encVersion);
            var sortedSavePath = await _sortService.SortAsync(realFileName);
            var revSortedSavePath = await _sortService.SortAsync(realFileName, revision);

            var encrypted = encVersion != AssetCrypt.EncVersion.NoEnc;

            if (File.Exists(revSortedSavePath) && !overWrite)
            {
                return _infoProvider.ProvideAssetInfo(fileName, realFileName, savePath, revSortedSavePath, encrypted);
            }

            if (!encrypted)
            {
                File.Copy(savePath, sortedSavePath, true);
                return _infoProvider.ProvideAssetInfo(fileName, realFileName, savePath, sortedSavePath, false);
            }

            try
            {
                await AssetCryptHelper.DecryptAsync(savePath, sortedSavePath, encVersion,
                    AssetCryptHelper.IdentifyShouldLz4(realFileName));
            }
            catch (Exception e)
            {
                // if the enc file can not be decrypt,
                // then directly copy it to the sorted directory.
                Logger?.LogError(e.ToString());
                realFileName = fileName;
                sortedSavePath = await _sortService.SortAsync(realFileName);
                revSortedSavePath = await _sortService.SortAsync(realFileName);
                File.Copy(savePath, sortedSavePath, true);
            }

            if (!string.Equals(revSortedSavePath,sortedSavePath))
            {
                File.Copy(sortedSavePath, revSortedSavePath, true);
            }

            return _infoProvider.ProvideAssetInfo(fileName, realFileName, savePath, revSortedSavePath, true);
        }
    }
}