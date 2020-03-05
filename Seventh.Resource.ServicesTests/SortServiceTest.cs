using Seventh.Resource.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
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
            var s = sortService.SortAsync(fileName,399).Result;
        }
    }
}
