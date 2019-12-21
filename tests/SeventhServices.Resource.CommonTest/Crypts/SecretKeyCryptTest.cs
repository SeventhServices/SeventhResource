using SeventhServices.Resource.Common.Crypts;
using Xunit;

namespace SeventhServices.Resource.CommonTest.Crypts
{
    public class SecretKeyCryptTest
    {
        [Fact]
        public void ShouldCrypt()
        {
            var encryptKey = SecretKeyCrypt.Encrypt("SeventhServices");
            var key = SecretKeyCrypt.Decrypt(encryptKey);
            Assert.Equal("SeventhServices", key);
        }
    }
}