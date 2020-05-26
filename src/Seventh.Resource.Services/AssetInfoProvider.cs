using System.IO;
using System.Security.Cryptography.X509Certificates;
using Seventh.Resource.Common.Entities;

namespace Seventh.Resource.Services
{
    public class AssetInfoProvider
    {
        public AssetInfo ProvideAssetInfo(string fileName, string sortedFileName, 
            string savePath, string sortedSavePath, bool encrypted)
        {
            var size = File.Exists(savePath) ? new FileInfo(savePath).Length : 0;
            var sortedSize = File.Exists(sortedSavePath) ? new FileInfo(sortedSavePath).Length : 0;
            return ProvideAssetInfo(
                fileName, sortedFileName, 
                size, sortedSize, 
                savePath, sortedSavePath, 
                encrypted);
        }

        public AssetInfo ProvideAssetInfo(string fileName, string sortedFileName, 
            long size, long sortedSize ,string savePath, string sortedSavePath, bool encrypted)
        {
            return new AssetInfo().InitialAsset(
                mirrorFileInfo : new AssetFileInfo
                {
                    Name = fileName,
                    Size = size,
                    Path = savePath
                },
                sortedFileInfo: new AssetFileInfo
                {
                    Name = sortedFileName,
                    Size = sortedSize,
                    Path = sortedSavePath
                },
                encrypted: encrypted
            );
        }
    }
}