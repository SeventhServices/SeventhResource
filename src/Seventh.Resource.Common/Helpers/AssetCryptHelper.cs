using System;
using System.IO;
using System.Threading.Tasks;
using Seventh.Resource.Common.Crypts;
using Seventh.Resource.Common.Extensions;

namespace Seventh.Resource.Common.Helpers
{
    public static class AssetCryptHelper
    {
        public static async Task DecryptAtDirectoryAsync(string filePath, string saveDirectory, AssetCrypt.EncVersion encVersion, bool lz4)
        {
            var fileName = Path.GetFileName(filePath);
            await DecryptAsync(filePath, saveDirectory.AppendPath(Rename(fileName, encVersion)), encVersion, lz4)
                .ConfigureAwait(false);
        }

        public static async Task DecryptWithRenameAsync(string filePath, string saveDirectory, bool lz4)
        {
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));
            var encVersion = AssetCrypt.IdentifyEncVersion(filePath);
            if (encVersion == AssetCrypt.EncVersion.NoEnc)
            {
                return;
            }
            await DecryptAtDirectoryAsync(filePath, saveDirectory, encVersion, lz4)
                .ConfigureAwait(false);
        }

        public static async Task DecryptWithRenameAsync(string filePath, string saveDirectory)
        {
            await DecryptWithRenameAsync(filePath, saveDirectory, IdentifyShouldLz4(filePath))
                .ConfigureAwait(false);
        }

        public static async Task DecryptAsync(string filePath, string savePath)
        {
            await DecryptAsync(filePath, savePath, AssetCrypt.IdentifyEncVersion(filePath), IdentifyShouldLz4(filePath))
                .ConfigureAwait(false);
        }

        public static async Task DecryptAsync(string filePath, string savePath, bool lz4)
        {
            await DecryptAsync(filePath, savePath, AssetCrypt.IdentifyEncVersion(filePath), lz4)
                .ConfigureAwait(false);
        }

        public static async Task DecryptAsync(string filePath, string savePath, AssetCrypt.EncVersion encVersion, bool lz4)
        {
            var fileBytes = await File.ReadAllBytesAsync(filePath)
                .ConfigureAwait(false);
            await using var fileStream = File.OpenWrite(savePath);
            await fileStream.WriteAsync(
                AssetCrypt.Decrypt(fileBytes, lz4, encVersion));
            fileStream.Close();
        }

        public static string Rename(string filePath)
        {
            return Rename(filePath, AssetCrypt.IdentifyEncVersion(filePath));
        }

        public static string Rename(string filePath, AssetCrypt.EncVersion encVersion)
        {
            var newFileName =
                encVersion == AssetCrypt.EncVersion.Ver2
                    ? AssetCrypt.ConvertFileName(filePath,
                        AssetCrypt.EncVersion.Ver2,
                        AssetCrypt.EncVersion.Ver1)
                    : filePath;

            return newFileName.Remove(newFileName.Length - 4);
        }

        public static bool IdentifyShouldLz4(string filePath)
        {
            if (filePath == null)
            {
                return false;
            }

            var fileType = filePath.Split('.');
            var type = fileType[^2];

            return Equals(type, "txt") ||
                   Equals(type, "sql") ||
                   Equals(type, "json");
        }
    }
}