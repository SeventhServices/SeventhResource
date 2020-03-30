using System.Collections.Generic;
using System.Linq;
using Seventh.Resource.Database.Abstractions;

namespace Seventh.Resource.Database.Serializer
{
    public static class SqlSerializer
    {
        private static readonly SqlRegexParser _sqlRegexParser = new SqlRegexParser();

        public static IEnumerable<T> Deserialize<T>(string sqlString) where T : class
        {
            return _sqlRegexParser.Parse<T>(sqlString).ToArray();
        }

        public static IEnumerable<T> DeserializeAsync<T>(string sqlString) where T : class
        {
            return _sqlRegexParser.Parse<T>(sqlString);
        }

        public static ISqlParser<T> GetDefault<T>() where T : class
        {
            return new SqlParser<T>();
        }

        public static SqlRegexParser GetRegexParser()
        {
            return _sqlRegexParser;
        }
    }
}