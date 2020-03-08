using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using LZ4;
using Seventh.Resource.Common.Utilities;

namespace Seventh.Resource.Common.Crypts
{
    public static class AssetCrypt
    {
        private const int SignatureSize = 7;
        private const int IvSize = 16;
        private static readonly byte[] PrivateKey = CommonUtil.ConvertByte(SecretKey.Implement.DecryptAssetPrivateKey);
        private static readonly byte[] PrivateKey2 = CommonUtil.ConvertByte(SecretKey.Implement.DecryptAssetPrivateKey2);
        private static readonly byte[] FileSignature = CommonUtil.ConvertByte("t7s-enc");
        private static byte[] _shuffledKey = new byte[IvSize];
        private static readonly HMACSHA256 HmacSha256 = new HMACSHA256(CommonUtil.ConvertByte(SecretKey.Implement.DecryptAssetNameHashKey));
        private static readonly Regex IdentifyEncVersionRegex = new Regex(@"[A-F0-9]{64}");

        public enum EncVersion
        {
            NoEnc,
            Ver1,
            Ver2
        }


        public static byte[] Decrypt<T>(T data, bool lz4 = false, EncVersion encVersion = EncVersion.Ver1)
        {
            if (data == null) return null;

            var fileSigSize = SignatureSize; //"t7s-enc",size =7;
            if (encVersion == EncVersion.Ver1) fileSigSize = 0;

            var dataArray = CommonUtil.ConvertByte(data); //文件数据转换为数组
            if (dataArray.Length <= (16 | fileSigSize)) return null;

            var ivArray = new byte[16];
            var encryptedArray = new byte[dataArray.Length - fileSigSize - 16]; //文件数组 - 文件头(16) - 文件标签(7)
            Buffer.BlockCopy(dataArray, fileSigSize, ivArray, 0, 16);
            Buffer.BlockCopy(dataArray, fileSigSize | 16, encryptedArray, 0, dataArray.Length - fileSigSize - 16);
            byte[] decryptedArray;
            if (encVersion == EncVersion.Ver1)
            {
                using var rhinelandManaged = new RijndaelManaged
                {
                    BlockSize = 128,
                    KeySize = 128,
                    IV = ivArray,
                    Key = PrivateKey,
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7
                };
                using var cryptoTransform = rhinelandManaged.CreateDecryptor();
                decryptedArray = cryptoTransform.TransformFinalBlock(
                    encryptedArray, 0, encryptedArray.Length);
            }
            else
            {
                SetShuffledKey(PrivateKey2);
                using var rhinelandManaged = new RijndaelManaged
                {
                    BlockSize = 128,
                    KeySize = 128,
                    IV = ivArray,
                    Key = _shuffledKey,
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7
                };
                using var cryptoTransform = rhinelandManaged.CreateDecryptor();
                decryptedArray = cryptoTransform.TransformFinalBlock(encryptedArray, 0, encryptedArray.Length);
                ClearShuffledKey();
            }

            return !lz4 ? decryptedArray : LZ4Codec.Decode(decryptedArray, 4, decryptedArray.Length - 4, BitConverter.ToInt32(decryptedArray, 0));
        }

        public static byte[] Encrypt<T>(T data, bool lz4 = false, EncVersion encVersion = EncVersion.Ver1)
        {
            if (data == null) return null;

            var dataArray = CommonUtil.ConvertByte(data);
            if (lz4)
            {
                var lz4Array = LZ4Codec.EncodeHC(dataArray, 0, dataArray.Length);
                if (lz4Array.Length > 0)
                {
                    var value = dataArray.Length;
                    dataArray = new byte[lz4Array.Length + 4];
                    Buffer.BlockCopy(BitConverter.GetBytes(value), 0, dataArray, 0, 4);
                    Buffer.BlockCopy(lz4Array, 0, dataArray, 4, lz4Array.Length);
                }
                else
                {
                    Console.WriteLine("Encrypt failed : " + data);
                }
            }

            if (encVersion == EncVersion.Ver1)
            {
                using var rhinelandManaged = new RijndaelManaged
                {
                    BlockSize = 128,
                    KeySize = 128,
                    Key = PrivateKey,
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7
                };
                rhinelandManaged.GenerateIV();
                var iv = rhinelandManaged.IV;
                using var cryptoTransform = rhinelandManaged.CreateEncryptor();
                var encryptedDataArray = cryptoTransform.TransformFinalBlock(dataArray, 0, dataArray.Length);

                var encryptedArray = new byte[iv.Length + encryptedDataArray.Length];
                Buffer.BlockCopy(iv, 0, encryptedArray, 0, 16);
                Buffer.BlockCopy(encryptedDataArray, 0, encryptedArray, 16, encryptedDataArray.Length);
                return encryptedArray;
            }
            else
            {
                SetShuffledKey(PrivateKey2);
                using var rhinelandManaged = new RijndaelManaged
                {
                    BlockSize = 128,
                    KeySize = 128,
                    Key = _shuffledKey,
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7
                };
                ClearShuffledKey();
                rhinelandManaged.GenerateIV();
                var iv = rhinelandManaged.IV;
                using var cryptoTransform = rhinelandManaged.CreateEncryptor();
                var encryptedDataArray = cryptoTransform.TransformFinalBlock(dataArray, 0, dataArray.Length);

                var encryptedArray = new byte[SignatureSize + iv.Length + encryptedDataArray.Length];
                Buffer.BlockCopy(iv, 0, encryptedArray, SignatureSize, 16);
                Buffer.BlockCopy(encryptedDataArray, 0, encryptedArray, SignatureSize | 16, encryptedDataArray.Length);
                Buffer.BlockCopy(FileSignature, 0, encryptedArray, 0, SignatureSize);
                return encryptedArray;
            }

        }

        private static void SetShuffledKey(byte[] rawKey)
        {
            var seed = 37;
            var array = _shuffledKey;
            Array.Copy(rawKey, array, rawKey.Length); //将raw移动到新数组
            var array2 = new byte[_shuffledKey.Length];
            var nowPosition = 0;
            foreach (var a in array)
            {
                array2[nowPosition] = Convert.ToByte(a ^ seed); //rawKe数组的每一个数与 seed 按位异或运算
                seed += 13;
                nowPosition++;
            }

            _shuffledKey = array2;
        }

        private static void ClearShuffledKey()
        {
            _shuffledKey = new byte[IvSize];
        }

        public static string ConvertFileName(string filePath, EncVersion versionFrom, EncVersion versionTo)
        {
            if (versionFrom == versionTo) return filePath;

            if (versionFrom == EncVersion.Ver1)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                var fileExtension = Path.GetExtension(fileName);
                var fileNameBytes = Encoding.UTF8.GetBytes(fileNameWithoutExtension ?? string.Empty);
                var hashBytes = HmacSha256.ComputeHash(fileNameBytes);
                var fileHash = BitConverter.ToString(hashBytes, 0, hashBytes.Length)
                    .Replace("-", string.Empty, StringComparison.Ordinal);

                return string.Concat(fileNameWithoutExtension,"_",fileHash,fileExtension);
            }
            else
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var fileExtension = Path.GetExtension(fileName);
                if (fileName == null) return null;
                var tempFileName = fileName.Split('_');
                var exportFileName = string.Join("_",
                    tempFileName.Take(tempFileName.Length - 1));

                return string.Concat(exportFileName,fileExtension,".enc");
            }
        }

        public static EncVersion IdentifyEncVersion(string fileName)
        {
            if (fileName == null)
            {
                return EncVersion.NoEnc;
            }
            if (!Path.GetExtension(fileName).Contains("enc", StringComparison.CurrentCulture))
            {
                return EncVersion.NoEnc;
            }

            return IdentifyEncVersionRegex.IsMatch(fileName) ? EncVersion.Ver2 : EncVersion.Ver1;
        }
    }
}