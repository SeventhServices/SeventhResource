using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Seventh.Resource.Common.Crypts;

namespace Seventh.Resource.Common.Helpers
{
    public static class FileNameConverter
    {
        public static string ToHashName(string fileName)
        {
#pragma warning disable CA5350 // 不要使用弱加密算法
            using var sha1 = SHA1.Create();
#pragma warning restore CA5350 // 不要使用弱加密算法
            var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(fileName));
            return BitConverter.ToString(hashBytes, 0, hashBytes.Length)
                .Replace("-", string.Empty, StringComparison.Ordinal);
        }

        public static string ToWithHashName(string fileName)
        {
            return AssetCrypt.ConvertFileName(fileName,
                AssetCrypt.EncVersion.Ver1,
                AssetCrypt.EncVersion.Ver2);
        }

        public static string ToLargeCardFile(int cardId)
        {
            return ToWithHashName($"card_l_{cardId.ToString("D5", CultureInfo.CurrentCulture)}.jpg.enc");
        }
    }
}