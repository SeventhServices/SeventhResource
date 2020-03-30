using System.Linq;

namespace Seventh.Resource.Database.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// 扩展方法，修正命名规范 card_id => CardId
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SnakeToCamel(this string str)
        {
            return str.Split(new[]
            {
                '_'
            }).Aggregate(string.Empty, (buffer, data)
                => buffer + char.ToUpper(data[0]) + data.Substring(1));
        }
    }
}