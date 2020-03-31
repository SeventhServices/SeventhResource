using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Seventh.Resource.Common.Options;
using Seventh.Resource.Data.Abstractions;
using Seventh.Resource.Database.Extensions;
using Seventh.Resource.Database.Serializer;
using Seventh.Resource.Services;

namespace Seventh.Resource.Database
{
    public class SqlLoader : ISqlLoader
    {
        private readonly AssetInfoService _assetInfoService;
        private readonly ILogger<SqlLoader> _logger;
        private readonly PathOption _pathOption;

        private readonly string DafaultClassName = "sql";
        public string RootPath => Path.Combine(_pathOption.AssetPath.SortedAssetPath, DafaultClassName);

        public SqlLoader(AssetInfoService assetInfoService, 
            ResourceLocation resourceLocation,
            ILogger<SqlLoader> logger)
        {
            _assetInfoService = assetInfoService;
            _logger = logger;
            _pathOption = resourceLocation.PathOption;
        }

        public async Task<string> TryGetLoadPathAsync<T>() where T : class
        {
            var fileInfos = await _assetInfoService.TryGetFileInfoByClassAsync(DafaultClassName);

            var info = fileInfos.FirstOrDefault(f => f.Revision == 0 &&
                         Path.GetFileNameWithoutExtension(f.Name)
                         .Replace("m_",string.Empty).SnakeToCamel()
                         .Equals(typeof(T).Name));

            _logger.LogInformation("Try load type {0} result is {1}", typeof(T).Name, info == null);

            return info == null ? null : Path.Combine(_pathOption.RootPath, info.Path);
        }

        public async Task<IEnumerable<T>> TryLoadAsync<T>() where T : class
        {
            var path = await TryGetLoadPathAsync<T>();
            return path == null ? null : await LoadAsync<T>(path);
        }

        public static IEnumerable<T> TryLoad<T>(string directory) where T : class
        {
            var filePath = new DirectoryInfo(directory).GetFiles()
                .FirstOrDefault(f => string.Concat(f.Name.Split('_')[1..^1])
                            == $"{typeof(T).Name.ToLower()}")?.FullName;

            return filePath == null ? null : Load<T>(filePath);
        }

        public static IEnumerable<T> Load<T>(string path) where T : class
        {
            Debug.WriteLine($"Load path {path}");
            return SqlSerializer.Deserialize<T>(File.ReadAllText(path));
        }

        public static async Task<IEnumerable<T>> LoadAsync<T>(string path) where T : class
        {
            Debug.WriteLine($"Load path {path}");
            return SqlSerializer.DeserializeAsync<T>(await File.ReadAllTextAsync(path));
        }
    }
}
