using System;
using System.Security.Cryptography;
using System.Text;
using SeventhServices.Resource.Common.Utilities;

namespace SeventhServices.Resource.Common.Crypts
{
    public static class SecretKeyCrypt
    {
        private static readonly RijndaelManaged Managed = new RijndaelManaged();

        static SecretKeyCrypt()
        {
            Managed.Key = Encoding.UTF8.GetBytes("777_STAR_SISTERS");
            Managed.IV = Managed.Key;
            Managed.Mode = CipherMode.CBC;
            Managed.Padding = PaddingMode.PKCS7;
        }

        public static string Encrypt(string key)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            using var cryptoTransform = Managed.CreateEncryptor();

            var encryptedKeyBytes = cryptoTransform.TransformFinalBlock(
                keyBytes, 0, keyBytes.Length);

            var stringBuilder = new StringBuilder();
            foreach (var b in encryptedKeyBytes)
            {
                stringBuilder.Append(Convert.ToString(b, 16)
                    .PadLeft(2,'0'));
            }

            return stringBuilder.ToString();
        }

        public static string Decrypt(string encryptedKey)
        {
            var encryptedKeyBytes = CommonUtil.ConvertHexStringToByte(encryptedKey);
            using var cryptoTransform = Managed.CreateDecryptor();

            var keyBytes = cryptoTransform.TransformFinalBlock(
                encryptedKeyBytes, 0, 
                encryptedKeyBytes.Length);
            return Encoding.UTF8.GetString(keyBytes);
        }

    }
}