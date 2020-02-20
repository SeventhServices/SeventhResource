using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Seventh.Resource.Common.Utilities
{
    public static class CommonUtil
    {
        public static byte[] ConvertHexStringToByte(string hex)
        {
            if (hex == null)
            {
                return null;
            }
            var length = hex.Length;
            var array = new byte[length / 2];
            for (var i = 0; i < length; i += 2) array[i / 2] =
                Convert.ToByte(hex.Substring(i, 2), 16);
            return array;
        }

        public static int Min(int a, int b)
        {
            return a >= b ? b : a;
        }

        public static byte[] ConvertByte(object data)
        {
            if (data == null)
            {
                return null;
            }
            var type = data.GetType();
            if (type == typeof(int)) return BitConverter.GetBytes((int)data);

            if (type == typeof(float)) return BitConverter.GetBytes((float)data);

            if (type == typeof(string)) return Encoding.UTF8.GetBytes(data.ToString());

            if (type == typeof(byte[])) return (byte[])data;

            return null;
        }

        public static string CreateNonexistentDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public static void CreateRequireDirectories<T>(T pathOption)
        {
            var properties = pathOption.GetType().GetProperties();

            foreach (var property in properties)
            {
                var path = property.GetValue(pathOption).ToString();
                Debug.Print($"Check path exist: {path}");
                CreateNonexistentDirectory(path);
            }
        }
    }
}