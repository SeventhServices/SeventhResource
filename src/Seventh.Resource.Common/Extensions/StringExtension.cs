using System;
using System.IO;
using Seventh.Resource.Common.Utilities;

namespace Seventh.Resource.Common.Extensions
{
    public static class StringExtension
    {
        public static T ToEnum<T>(this string str) where T : struct, IComparable, IFormattable, IConvertible
        {
            return (T)Enum.Parse(typeof(T), str);
        }
        public static string AppendPath(this string basePath, string path)
        {
            return Path.Combine(basePath, path);
        }
        public static string AppendAndCreatePath(this string basePath, string path)
        {
            return CommonUtil.CreateNonexistentDirectory(AppendPath(basePath, path));
        }
        public static string AppendPath(this string basePath, string path1, string path2)
        {
            return Path.Combine(basePath, path1, path2);
        }
        public static string AppendAndCreatePath(this string basePath, string path1, string path2)
        {
            return CommonUtil.CreateNonexistentDirectory(AppendPath(basePath, path1, path2));
        }
    }
}