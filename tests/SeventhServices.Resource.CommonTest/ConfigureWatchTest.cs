using System.IO;
using System.Threading.Tasks;
using SeventhServices.Resource.Common.Utilities;
using SeventhServices.Resource.CommonTest.Comparer;
using SeventhServices.Resource.CommonTest.TestClasses;
using Xunit;

namespace SeventhServices.Resource.CommonTest
{
    public class ConfigureWatchTest
    {
        [Fact]
        public void ShouldAdd()
        {
            ConfigureWatcher.RemoveConfigure<TestConfigure>();
            ConfigureWatcher.TryAddConfigure<TestConfigure>();

            Assert.True(File.Exists(
                ConfigureWatcher.GetSaveFilePath<TestConfigure>()));

            var newTestConfigure = ConfigureWatcher.GetConfigure<TestConfigure>();
            Assert.Equal(new TestConfigure(), newTestConfigure, 
                new ClassPropertyComparer( new []{ "LastModify" }));
        }

        [Fact]
        public void ShouldRefresh()
        {
            ConfigureWatcher.RemoveConfigure<TestConfigure>();
            var testConfigure = new TestConfigure();
            ConfigureWatcher.TryAddConfigure<TestConfigure>();
            ConfigureWatcher.RefreshConfigure<TestConfigure>(testConfigure);

            Assert.True(File.Exists(
                ConfigureWatcher.GetSaveFilePath<TestConfigure>()));

            testConfigure.Version = "6.8.1";

            ConfigureWatcher.RefreshConfigure<TestConfigure>(testConfigure);

            var newTestConfigure = ConfigureWatcher.GetConfigure<TestConfigure>();
            Assert.Equal(testConfigure, newTestConfigure, 
                new ClassPropertyComparer());
        }

        [Fact]
        public void ShouldAddExist()
        {
            ConfigureWatcher.RemoveConfigure<TestConfigure>();
            var testConfigure = ConfigureWatcher.TryAddConfigure<TestConfigure>();
            testConfigure.Version = "7.0.0";
            ConfigureWatcher.RefreshConfigure<TestConfigure>(testConfigure);

            Assert.True(File.Exists(
                ConfigureWatcher.GetSaveFilePath<TestConfigure>()));

            ConfigureWatcher.TryAddConfigure<TestConfigure>();

            var newTestConfigure = ConfigureWatcher.GetConfigure<TestConfigure>();
            Assert.Equal(testConfigure, newTestConfigure, new ClassPropertyComparer());
        }

        [Fact]
        public async Task ShouldAddAsync()
        {
            ConfigureWatcher.RemoveConfigure<TestAddAsyncConfigure>();
            await ConfigureWatcher.TryAddConfigureAsync<TestAddAsyncConfigure>();

            Assert.True(File.Exists(
                ConfigureWatcher.GetSaveFilePath<TestAddAsyncConfigure>()));

            var newTestConfigure = ConfigureWatcher.GetConfigure<TestAddAsyncConfigure>();
            Assert.Equal(new TestAddAsyncConfigure(), newTestConfigure, 
                new ClassPropertyComparer(new[] { "LastModify" }));
        }

        [Fact]
        public async Task ShouldRefreshAsync()
        {
            ConfigureWatcher.RemoveConfigure<TestRefreshAsyncConfigure>();
            var testConfigure = new TestRefreshAsyncConfigure();
            await ConfigureWatcher.TryAddConfigureAsync<TestRefreshAsyncConfigure>();
            await ConfigureWatcher.RefreshConfigureAsync<TestRefreshAsyncConfigure>();

            Assert.True(File.Exists(
                ConfigureWatcher.GetSaveFilePath<TestRefreshAsyncConfigure>()));

            testConfigure.Version = "6.8.1";

            await ConfigureWatcher.RefreshConfigureAsync<TestRefreshAsyncConfigure>(testConfigure);

            var newTestConfigure = ConfigureWatcher.GetConfigure<TestRefreshAsyncConfigure>();
            Assert.Equal(testConfigure, newTestConfigure, new ClassPropertyComparer());
        }

        [Fact]
        public async Task ShouldAddExistAsync()
        {
            ConfigureWatcher.RemoveConfigure<TestAddExistAsyncConfigure>();
            var testAddExistAsyncConfigure = await ConfigureWatcher.TryAddConfigureAsync<TestAddExistAsyncConfigure>();
            testAddExistAsyncConfigure.Version = "7.0.0";
            await ConfigureWatcher.RefreshConfigureAsync<TestAddExistAsyncConfigure>(testAddExistAsyncConfigure);

            Assert.True(File.Exists(
                ConfigureWatcher.GetSaveFilePath<TestAddExistAsyncConfigure>()));
                
            var newAddExistAsyncConfigure = await ConfigureWatcher.TryAddConfigureAsync<TestAddExistAsyncConfigure>();

            Assert.Equal(testAddExistAsyncConfigure, 
                newAddExistAsyncConfigure, new ClassPropertyComparer());
        }
    }
}
