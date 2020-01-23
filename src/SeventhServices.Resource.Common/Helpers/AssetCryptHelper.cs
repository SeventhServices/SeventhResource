using System;
using System.IO;
using System.Threading.Tasks;
using SeventhServices.Resource.Common.Crypts;
using SeventhServices.Resource.Common.Extensions;

namespace SeventhServices.Resource.Common.Helpers
{
    public static class AssetCryptHelper
    {
        public static async Task DecryptWithRename(string filePath, string saveDirectory, bool lz4 = false)
        {
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));

            var fileName = Path.GetFileName(filePath);
            var encVersion = AssetCrypt.IdentifyEncVersion(fileName);
            if (encVersion == AssetCrypt.EncVersion.NoEnc)
            {
                return;
            }
            var fileBytes = File.ReadAllBytes(filePath);
            await using var fileStream = File.OpenWrite(saveDirectory.AppendPath(Rename(fileName, encVersion)));
            await fileStream.WriteAsync(
                AssetCrypt.Decrypt(fileBytes, lz4, encVersion));
            fileStream.Close();
        }

        public static string Rename(string fileName)
        {
            return Rename(fileName, AssetCrypt.IdentifyEncVersion(fileName));
        }

        public static string Rename(string fileName, AssetCrypt.EncVersion encVersion)
        {
            var newFileName =
                encVersion == AssetCrypt.EncVersion.Ver2
                    ? AssetCrypt.ConvertFileName(fileName,
                        AssetCrypt.EncVersion.Ver2,
                        AssetCrypt.EncVersion.Ver1)
                    : fileName;

            return newFileName.Remove(newFileName.Length - 4);
        }
    }
}