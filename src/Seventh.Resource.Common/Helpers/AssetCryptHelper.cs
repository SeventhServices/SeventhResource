using System;
using System.IO;
using System.Threading.Tasks;
using Seventh.Resource.Common.Crypts;
using Seventh.Resource.Common.Extensions;

namespace Seventh.Resource.Common.Helpers
{
    public static class AssetCryptHelper
    {
        public static async Task DecryptWithRename(string filePath, string saveDirectory)
        {
            await DecryptWithRename(filePath, saveDirectory, IdentifyShouldLz4(filePath))
                .ConfigureAwait(false);
        }

        public static async Task DecryptWithRename(string filePath, string saveDirectory, bool lz4)
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