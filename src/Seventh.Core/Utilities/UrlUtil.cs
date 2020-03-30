using System;
using System.IO;
using System.Linq;

namespace Seventh.Core.Utilities
{
    public static class UrlUtil
    {
        public static string MakeFileUrl(string baseUrl, string filePath)
        {
            var directories = filePath.Split(Path.DirectorySeparatorChar,
                StringSplitOptions.RemoveEmptyEntries);
            var fileName = directories.Last();
            var url = directories.SkipLast(1).Select(
                d => d.ToLowerInvariant()).ToArray();
            if (url.Length == 0)
            {
                return string.Concat(baseUrl, fileName);
            }
            return string.Concat(baseUrl, string.Join("/", url), "/", fileName);
        }
    }
}