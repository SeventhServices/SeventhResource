using System;
using System.IO;
using Seventh.Resource.Common.Entities;
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

            var filePath = AccountHelper.GetExportAccountFilePath("7901801", "");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var testAccount = new Account("7901801", uuid.ToString(), false);
            AccountHelper.ExportToFile(testAccount, "");

            Assert.True(File.Exists(filePath));

            var inputAccount = AccountHelper.ReadFromFile("7901801", "");

            Assert.Equal(inputAccount, testAccount);
        }
    }
}