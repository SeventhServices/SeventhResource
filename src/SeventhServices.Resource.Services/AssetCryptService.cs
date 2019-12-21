using System.IO;
using System.Threading.Tasks;
using SeventhServices.Resource.Common.Crypts;
using SeventhServices.Resource.Common.Extensions;

namespace SeventhServices.Resource.Services
{
    public class AssetCryptService
    {
        public async Task DecryptWithRename(string filePath, string saveDirectory, bool lz4 = false)
        {
            var fileName = Path.GetFileName(filePath);
            var encVersion = AssetCrypt.IdentifyEncVersion(fileName);
            if (encVersion == AssetCrypt.EncVersion.NoEnc)
            {
                return;
            }
            var fileBytes = File.ReadAllBytes(filePath);
            await using var fileStream = File.OpenWrite(saveDirectory.AppendPath(Rename(fileName,encVersion)));
            await fileStream.WriteAsync(
                AssetCrypt.Decrypt(fileBytes, lz4, encVersion));
            fileStream.Close();
        }

        public string Rename(string fileName, AssetCrypt.EncVersion encVersion)
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