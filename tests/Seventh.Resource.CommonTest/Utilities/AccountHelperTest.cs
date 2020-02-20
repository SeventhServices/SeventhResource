using System;
using System.IO;
using Seventh.Resource.Common.Classes;
using Seventh.Resource.Common.Helpers;
using Xunit;

namespace Seventh.Resource.CommonTest.Utilities
{
    public class AccountHelperTest
    {
        [Fact]
        public void ShouldConvert()
        {
            var uuid = Guid.NewGuid();
            AccountHelper.ConvertToFile(new Account("7901801", uuid.ToString(), false), Directory.GetCurrentDirectory());

            var filePath = AccountHelper.GetAccountFilePath("7901801", Directory.GetCurrentDirectory());
            Assert.True(File.Exists(filePath));
        }
    }
}