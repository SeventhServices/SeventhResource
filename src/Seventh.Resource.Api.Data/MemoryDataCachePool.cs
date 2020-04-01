using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Seventh.Resource.Api.Data.Abstractions;
using Seventh.Resource.Data.Abstractions;
using Seventh.Resource.Database;

namespace Seventh.Resource.Api.Data
{
    public class MemoryDataCachePool : IDataCachePool
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ISqlLoader _sqlLoader;
        private readonly PhysicalFileProvider _fileProvider;

        public MemoryDataCachePool(IMemoryCache memoryCache, ISqlLoader sqlLoader)
        {
            _memoryCache = memoryCache;
            _sqlLoader = sqlLoader;
            _fileProvider = new PhysicalFileProvider(_sqlLoader.RootPath);
        }

        public async Task<IEnumerable<T>> CreateOrGetAsync<T>() where T : class
        {
            var name = typeof(T).FullName;
            var path = await _sqlLoader.TryGetLoadPathAsync<T>();
            var fileName = Path.GetFileName(path);
            return await _memoryCache.GetOrCreateAsync(name,async e =>
            {
                e.AddExpirationToken(_fileProvider.Watch(fileName));
                return (await SqlLoader.LoadAsync<T>(path)).ToImmutableHashSet();
            });
        }
    }
}
