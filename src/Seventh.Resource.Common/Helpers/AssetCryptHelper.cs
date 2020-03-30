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
            await DecryptAtDirectoryAsync(filePath, saveDirectory, encVersion, lz4)
                .ConfigureAwait(false);
        }

        public static async Task DecryptWithRenameAsync(string filePath, string saveDirectory)
        {
            var fileName = Path.GetFileName(filePath);
            var encVersion = AssetCrypt.IdentifyEncVersion(filePath);
            await DecryptAsync(filePath, saveDirectory.AppendPath(Rename(fileName, encVersion)),
                    encVersion, IdentifyShouldLz4(fileName))
                .ConfigureAwait(false);
        }

        public static async Task DecryptAsync(string filePath, string savePath)
        {
            var encVersion = AssetCrypt.IdentifyEncVersion(filePath);
            await DecryptAsync(filePath, savePath, encVersion, IdentifyShouldLz4(filePath))
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
                AssetCrypt.Decrypt(fileBytes, lz4, encVersion))
                .ConfigureAwait(false);
            fileStream.Close();
        }

        public static string Rename(string filePath)
        {
            return Rename(filePath, AssetCrypt.IdentifyEncVersion(filePath));
        }

        public static string Rename(string filePath, AssetCrypt.EncVersion encVersion)
        {
            if (encVersion == AssetCrypt.EncVersion.NoEnc)
            {
                return filePath;
            }

            var encVersion1FilePath = AssetCrypt.ConvertFileName(filePath, encVersion, AssetCrypt.EncVersion.Ver1);

            return encVersion1FilePath?.Remove(encVersion1FilePath.Length - 4);
        }

        public static bool IdentifyShouldLz4(string filePath)
        {
            if (filePath == null) return false;

            var type = Path.GetExtension(filePath);

            return type.Equals(".txt", StringComparison.OrdinalIgnoreCase) ||
                   type.Equals(".sql", StringComparison.OrdinalIgnoreCase) ||
                   type.Equals(".json", StringComparison.OrdinalIgnoreCase);
        }
    }
}