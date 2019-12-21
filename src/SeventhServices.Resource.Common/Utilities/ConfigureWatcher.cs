using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using SeventhServices.Resource.Common.Abstractions;
using SeventhServices.Resource.Common.Extensions;

namespace SeventhServices.Resource.Common.Utilities
{
    public static class ConfigureWatcher
    {
        private static readonly Dictionary<Type, IConfigureFile> ConfigureFiles = new Dictionary<Type, IConfigureFile>();

        static ConfigureWatcher()
        {
            SavePath = Path.Combine(Directory.GetCurrentDirectory(), "Configuretions");
            CommonUtil.CreateNonexistentDirectory(SavePath);
        }

        private static void ThrowArgumentNullExceptionHelper(string message)
        {
            throw new ArgumentNullException(message);
        }

        public static string SavePath { get; set; }

        public static T GetFreshConfigure<T>() where T : IConfigureFile
        {
            RefreshConfigure<T>();
            return GetConfigure<T>();
        }

        public static T TryAddConfigure<T>()
            where T : IConfigureFile,new()
        {
            if (File.Exists(GetSaveFilePath<T>()))
            {
                ConfigureFiles.TryAdd(typeof(T), Read<T>());
            }
            else
            {
                RefreshConfigure<T>(new T());
            }
            return GetConfigure<T>();
        }

        public static T AddConfigure<T>(
            IConfigureFile configureFile)
            where T : IConfigureFile
        {
            if (configureFile == null)
            {
                ThrowArgumentNullExceptionHelper(typeof(T).Name);
                return default;
            }

            ConfigureFiles.Remove(typeof(T));
            ConfigureFiles.Add(typeof(T), configureFile);
            Save<T>(configureFile);
            return GetConfigure<T>();
        }

        public static T RefreshConfigure<T>(IConfigureFile configureFile)
            where T : IConfigureFile
        {
            if (configureFile == null)
            {
                ThrowArgumentNullExceptionHelper(typeof(T).Name);
                return default;
            }
            Save<T>(configureFile);
            return RefreshConfigure<T>();
        }

        public static T RefreshConfigure<T>()
            where T : IConfigureFile
        {
            ConfigureFiles.Remove(typeof(T));
            ConfigureFiles.Add(typeof(T), Read<T>());
            return GetConfigure<T>();
        }

        public static T GetConfigure<T>() 
            where T : IConfigureFile
        {
            ConfigureFiles.TryGetValue(typeof(T), out var configureFile);
            if (configureFile == null)
            {
                return default;
            }
            return (T)configureFile;
        }

        public static void RemoveConfigure<T>()
            where T : IConfigureFile
        {
            ConfigureFiles.Remove(typeof(T));
            File.Delete(GetSaveFilePath<T>());
        }

        private static T Read<T>()
        {
            return JsonSerializer.Deserialize<T>(
                File.ReadAllText(GetSaveFilePath<T>()));
        }

        private static void Save<T>(IConfigureFile file)
        {
            file.LastModify = DateTime.UtcNow;
            using var streamWriter = new StreamWriter(GetSaveFilePath<T>());
            var json = JsonSerializer.Serialize<T>((T)file,
                new JsonSerializerOptions { WriteIndented = true });
            streamWriter.Write(json);
            streamWriter.Close();
        }

        public static string GetSaveFilePath<T>()
        {
            return SavePath.AppendPath($"{typeof(T).Name}.json");
        }

        public static async Task<T> GetFreshConfigureAsync<T>() 
            where T : IConfigureFile
        {
            await RefreshConfigureAsync<T>()
                .ConfigureAwait(false);
            return GetConfigure<T>();
        }

        public static async Task<T> TryAddConfigureAsync<T>()
            where T : IConfigureFile,new()
        {
            if (File.Exists(GetSaveFilePath<T>()))
            {
                ConfigureFiles.TryAdd(typeof(T), Read<T>());
            }
            else
            {
                await RefreshConfigureAsync<T>(new T())
                    .ConfigureAwait(false);
            }

            return GetConfigure<T>();
        }

        public static async Task<T> AddConfigureAsync<T>(IConfigureFile configureFile)
            where T : IConfigureFile
        {
            if (configureFile == null)
            {
                ThrowArgumentNullExceptionHelper(typeof(T).Name);
                return default;
            }

            ConfigureFiles.Remove(typeof(T));
            ConfigureFiles.Add(typeof(T), configureFile);
            await SaveAsync<T>(configureFile)
                .ConfigureAwait(false);
            return GetConfigure<T>();
        }

        public static async Task<T> RefreshConfigureAsync<T>(IConfigureFile configureFile)
            where T : IConfigureFile
        {
            if (configureFile == null)
            {
                return GetConfigure<T>();
            }
            Save<T>(configureFile);
            return await RefreshConfigureAsync<T>()
                .ConfigureAwait(false);
        }

        public static async Task<T> RefreshConfigureAsync<T>()
            where T : IConfigureFile
        {
            ConfigureFiles.Remove(typeof(T));
            ConfigureFiles.Add(typeof(T), 
                await ReadAsync<T>()
                .ConfigureAwait(false));
            return GetConfigure<T>();
        }

        private static async Task<T> ReadAsync<T>()
        {
            await using var fileStream = File.OpenRead(GetSaveFilePath<T>());
            var configure = await JsonSerializer.DeserializeAsync<T>(fileStream);
            fileStream.Close();
            return configure;
        }

        private static async Task SaveAsync<T>(IConfigureFile file)
        {
            file.LastModify = DateTime.UtcNow;
            await using var fileStream = File.Open(GetSaveFilePath<T>(),
                FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            await JsonSerializer.SerializeAsync<T>(fileStream, (T)file,
                    new JsonSerializerOptions { WriteIndented = true })
                .ConfigureAwait(false);
            fileStream.Close();
        }
    }
}