using System.Collections.Generic;

namespace Seventh.Resource.Database.Abstractions
{
    public interface ISqlParser<out T> where T : class
    {
        public IEnumerable<T> Parse(string sqlString);
    }
}