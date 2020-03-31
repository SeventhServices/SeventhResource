using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Seventh.Resource.Api.Data.Abstractions
{
    public interface IRepository<T> where T : class
    {
        public Task<IEnumerable<T>> GetListAsync();
    }
}
