using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Seventh.Resource.Database.Extensions
{
    internal static class PathExtension
    {
        internal static string Local { get; set; } = Path.GetDirectoryName(Assembly.GetAssembly(typeof(PathExtension)).Location);

        internal static string GetGameSqlDirectory()
        {
            return Path.Combine(Local, "GameSql");
        }

        internal static IEnumerable<FileInfo> GetGameSqlFileInfos()
        {
            return new DirectoryInfo(GetGameSqlDirectory()).GetFiles();
        }
    }
}