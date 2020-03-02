using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

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
                d => d.ToLowerInvariant());
            return string.Concat(baseUrl,string.Join("/", url),"/",fileName); 
        }
    }
}