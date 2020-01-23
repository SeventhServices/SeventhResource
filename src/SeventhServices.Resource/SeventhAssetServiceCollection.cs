using System;
using Microsoft.Extensions.DependencyInjection;
using SeventhServices.Resource.Common.Classes.Options;
using SeventhServices.Resource.Services;

namespace SeventhServices.Resource
{
    public static class SeventhAssetServiceCollection
    {
        public static void AddSeventhAssetServices(this IServiceCollection services)
        {
            services.AddSingleton(p => new OptionService()
                .ConfigureOptions());
            services.AddSingleton<FileWatcherService>();
            services.AddSingleton<AssetDownloadService>();
        }

        public static void AddSeventhAssetServices(this IServiceCollection services, Action<ResourceOption> resourceOptions)
        {
            var resourceOption = new ResourceOption();
            resourceOptions(new ResourceOption());
            services.AddSingleton(p => new OptionService()
                .UseStatusOptions(resourceOption));
            services.AddSingleton<FileWatcherService>();
            services.AddSingleton<AssetDownloadService>();
        }
    }
}