using System.Collections.Generic;
using System.IO;
using System.Linq;
using Seventh.Resource.Database.Extensions;
using Seventh.Resource.Database.Serializer;

namespace Seventh.Resource.Database
{
    public class Loader
    {
        public static IEnumerable<T> TryLoad<T>() where T : class
        {
            var filePath = PathExtension.GetGameSqlFileInfos()
                .FirstOrDefault(f => string.Concat(f.Name.Split('_')[1..^1])
                            == $"{typeof(T).Name.ToLower()}")?.FullName;

            return filePath == null ? null : Load<T>(filePath);
        }

        public static IEnumerable<T> Load<T>(string path) where T : class
        {
            return SqlSerializer.Deserialize<T>(File.ReadAllText(path));
        }
    }
}
