using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Seventh.Resource.Database.Extensions;
using Seventh.Resource.Database.Serializer;

namespace Seventh.Resource.Database
{
    public class SqlLoader
    {
        public static IEnumerable<T> TryLoad<T>(string directory) where T : class
        {
            var filePath = new DirectoryInfo(directory).GetFiles()
                .FirstOrDefault(f => string.Concat(f.Name.Split('_')[1..^1])
                            == $"{typeof(T).Name.ToLower()}")?.FullName;

            return filePath == null ? null : Load<T>(filePath);
        }

        public static IEnumerable<T> Load<T>(string path) where T : class
        {
            return SqlSerializer.Deserialize<T>(File.ReadAllText(path));
        }

        public static async Task<IEnumerable<T>> LoadAsync<T>(string path) where T : class
        {
            return SqlSerializer.DeserializeAsync<T>(await File.ReadAllTextAsync(path));
        }
    }
}
