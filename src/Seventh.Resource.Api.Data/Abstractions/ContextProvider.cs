using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Seventh.Resource.Api.Data.Abstractions
{
    public interface IContextProvider
    {
        public Task<IEnumerable<T>> ProvideAsync<T>() where T : class;
    }
}
