using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Seventh.Resource.Common.Utilities;

namespace Seventh.Resource.Common.Crypts
{
    public static class AccountCrypt
    {
        private static readonly byte[] Rand = new byte[36]
{
            5,
            3,
            1,
            7,
            3,
            3,
            1,
            0,
            1,
            3,
            4,
            6,
            2,
            3,
            2,
            2,
            2,
            3,
            4,
            6,
            3,
            4,
            7,
            1,
            2,
            5,
            4,
            1,
            6,
            3,
            6,
            7,
            0,
            7,
            1,
            7
};

        public static string DecryptUuid(string encUuid, string iv)
        {
            return DecryptByTripleDes(SecretKey.Implement.DecryptUuidKey, iv, encUuid);
        }

        public static string DecryptByTripleDes(string key, string iv, string encryptedText)
        {
#pragma warning disable CA5350 // 不要使用弱加密算法
            using var tripleDes = TripleDES.Create();
#pragma warning restore CA5350 // 不要使用弱加密算法
            tripleDes.Key = Encoding.ASCII.GetBytes(key);
            tripleDes.IV = CommonUtil.ConvertHexStringToByte(iv);
            tripleDes.Mode = CipherMode.CBC;
            tripleDes.Padding = PaddingMode.Zeros;

            using var cryptoTransform = tripleDes.CreateDecryptor();
            var array = CommonUtil.ConvertHexStringToByte(encryptedText);
            var array2 = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
            var @string = Encoding.ASCII.GetString(array2, 0, array2.Length);
            return new string((from c in @string
                               where !char.IsControl(c)
                               select c).ToArray());
        }


        public static string Decrypt(string str)
        {
            if (str == null) { throw new ArgumentNullException(nameof(str)); }

            var array = new byte[str.Length / 2];
            for (var i = 0; i < CommonUtil.Min(array.Length, Rand.Length); i++)
            {
                var b = Convert.ToByte(str.Substring(i * 2, 2), 16);
                array[i] = (byte)(b ^ Rand[i]);
            }

            return Encoding.ASCII.GetString(array);
        }

        public static string Encrypt(string str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));

            var stringBuilder = new StringBuilder();
            for (var i = 0; i < CommonUtil.Min(str.Length, Rand.Length); i++)
                stringBuilder.Append(Convert.ToString(str[i] ^ Rand[i], 16));
            return stringBuilder.ToString();
        }
    }
}