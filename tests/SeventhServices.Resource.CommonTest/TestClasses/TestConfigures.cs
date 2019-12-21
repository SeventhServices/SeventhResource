using SeventhServices.Resource.Common.Abstractions;

namespace SeventhServices.Resource.CommonTest.TestClasses
{

    public class TestConfigure : ConfigureFile
    {
        public string Version { get; set; } = "6.8.1";
        public string Pid { get; set; }
        public string Uuid { get; set; }
        public int Rev { get; set; } = 368;
        public string Os { get; set; } = "android";
        public string Device { get; set; } = "Android";
        public string Platform { get; set; } = "xiaomi 9";
        public string OsVersion { get; set; } = "10";
        public string Jb { get; set; } = "0";
    }

    public class TestAddAsyncConfigure : TestConfigure { }

    public class TestRefreshAsyncConfigure : TestConfigure { }

    public class TestAddExistAsyncConfigure : TestConfigure { }
}