using System;

namespace Seventh.Core.Utilities
{
    public static class UrlUtil
    {
        public static string MakeFileUrl(string baseUrl, string filePath)
        {
            var directories = filePath.Split(new []{"\\","/"},StringSplitOptions.RemoveEmptyEntries);
            return string.Concat(baseUrl,string.Join("/", directories)); 
        }
    }
}