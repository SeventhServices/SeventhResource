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
            var fileName = "specialshop_menu_2245_a.png.enc";
            var s = sortService.SortAsync(fileName);
        }
    }
}
