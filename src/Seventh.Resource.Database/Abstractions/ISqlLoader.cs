using System.Collections.Generic;
using System.Threading.Tasks;

namespace Seventh.Resource.Data.Abstractions
{
    public interface ISqlLoader
    {
        public string RootPath { get; }
        public Task<string> TryGetLoadPathAsync<T>() where T : class;
        public Task<IEnumerable<T>> TryLoadAsync<T>() where T : class;
    }
}
