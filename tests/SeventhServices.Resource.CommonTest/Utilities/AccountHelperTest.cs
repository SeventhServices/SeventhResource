using System.IO;
using SeventhServices.Resource.Common.Classes;
using SeventhServices.Resource.Common.Helpers;
using SeventhServices.Resource.Common.Utilities;
using Xunit;

namespace SeventhServices.Resource.CommonTest.Utilities
{
    public class AccountHelperTest
    {
        [Fact]
        public void ShouldConvert()
        {
            AccountHelper.ConvertToFile(new Account("790180", "98c31889-8de0-4a2c-afe3-4f8338d1396e", false), Directory.GetCurrentDirectory());

            var filePath = AccountHelper.GetAccountFilePath("790180", Directory.GetCurrentDirectory());
            Assert.True(File.Exists(filePath));
        }
    }
}