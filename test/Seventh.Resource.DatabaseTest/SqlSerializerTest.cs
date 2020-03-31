using System.IO;
using System.Linq;
using Seventh.Resource.Database.Entity;
using Seventh.Resource.Database.Serializer;
using Xunit;

namespace Seventh.Resource.DatabaseTest
{
    public class SqlSerializerTest
    {

        [Fact]
        public void ShouldSerializer()
        {
            var cards = SqlSerializer.Deserialize<Card>(File.ReadAllText("test.sql")).ToArray();
            var cardss = cards.Where(c => c.Role != "0");
            Assert.True(cards.Length != 0);
        }
    }
}