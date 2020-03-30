using System.IO;
using Seventh.Resource.Common.Crypts;
using Xunit;

namespace Seventh.Resource.CommonTest.Crypts
{
    public class AssetCryptTest
    {
        [Fact]
        public void ShouldDecrypt()
        {
            var testFileBytes = File.ReadAllBytes(Path.Combine("TestExample", "test.bin"));
            var testEncryptedFileBytes = File.ReadAllBytes(Path.Combine("TestExample", "test.bin.enc"));
            var decryptedArray = AssetCrypt.Decrypt(testEncryptedFileBytes);
            Assert.Equal(testFileBytes, decryptedArray);
        }

        [Fact]
        public void ShouldEncrypt()
        {
            var testFileBytes = File.ReadAllBytes(Path.Combine("TestExample", "test.bin"));
            var encryptedArray = AssetCrypt.Encrypt(testFileBytes, true, AssetCrypt.EncVersion.Ver2);
            var decryptedArray = AssetCrypt.Decrypt(encryptedArray, true, AssetCrypt.EncVersion.Ver2);
            Assert.Equal(testFileBytes, decryptedArray);
        }
    }
}