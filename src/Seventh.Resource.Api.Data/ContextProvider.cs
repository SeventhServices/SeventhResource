using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Seventh.Resource.Api.Data.Abstractions
{
    public class ContextProvider : IContextProvider
    {
        private readonly IDataCachePool _cachePool;

        public ContextProvider(IDataCachePool cachePool)
        {
            _cachePool = cachePool;
        }

        public async Task<IEnumerable<T>> ProvideAsync<T>() 
            where T : class
        {
            return await _cachePool.CreateOrGetAsync<T>();
        }
    }
}
