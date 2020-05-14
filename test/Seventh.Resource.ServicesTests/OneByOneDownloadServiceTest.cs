using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Seventh.Resource.Services;
using Xunit;

namespace Seventh.Resource.ServicesTests
{
    public class OneByOneDownloadServiceTest
    {
        public OneByOneDownloadServiceTest()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.TryAddSingleton(p => new ResourceLocation()
                .ConfigureLocation());
            serviceCollection.TryAddSingleton<FileWatcherService>();
            serviceCollection.AddHttpClient<DownloadClient>();
            serviceCollection.TryAddScoped<DownloadService>();
            serviceCollection.AddHttpClient<OneByOneDownloadClient>();
            serviceCollection.TryAddScoped<OneByOneDownloadService>();
            serviceCollection.TryAddSingleton<SortService>();
            serviceCollection.TryAddSingleton<AssetInfoProvider>();
            _services = serviceCollection.BuildServiceProvider();
        }

        private readonly IServiceProvider _services;

        [Fact]
        public async Task ShouldDownloadLargeCard()
        {
            var oneByOneDownloadService = _services.CreateScope().ServiceProvider.GetService<OneByOneDownloadService>();
            var (result, info) = await oneByOneDownloadService.TryDownloadLargeCard(4000);
            Assert.True(result);
        }
    }
}