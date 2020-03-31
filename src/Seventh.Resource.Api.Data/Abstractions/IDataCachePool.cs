using System.Collections.Generic;
using System.Threading.Tasks;

namespace Seventh.Resource.Api.Data.Abstractions
{
    public interface IDataCachePool
    {
        public Task<IEnumerable<T>> CreateOrGetAsync<T>() where T : class;
    }
}
