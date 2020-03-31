using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Seventh.Resource.Api.Data.Abstractions;

namespace Seventh.Resource.Api.Data
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly Task<IEnumerable<T>> _context;

        public BaseRepository(IContextProvider provider)
        {
            _context = provider.ProvideAsync<T>();
        }
        public abstract Task<IEnumerable<T>> GetListAsync();
    }
}
