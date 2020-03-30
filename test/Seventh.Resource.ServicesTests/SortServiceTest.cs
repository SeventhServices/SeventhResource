using System.IO;
using Seventh.Resource.Services;
using Xunit;

namespace Seventh.Resource.ServicesTests
{
    public class SortServiceTest
    {
        [Fact]
        public void ShouldProvideExtendPath()
        {
            var sortService = new SortService(new ResourceLocation().ConfigureLocation());
            var fileName = "top.jpg";
            var sortedFileName = sortService.SortAsync(fileName, 399).Result;
            Assert.Equal("top_r399.jpg", Path.GetFileName(sortedFileName));
            fileName = "gacha_banner_7thaudition.png";
            sortedFileName = sortService.SortAsync(fileName, 399).Result;
            Assert.Equal("gacha_banner_7thaudition_r399.png", Path.GetFileName(sortedFileName));
        }
    }
}
