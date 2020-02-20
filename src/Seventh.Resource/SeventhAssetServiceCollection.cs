using System;
using Microsoft.Extensions.DependencyInjection;
using Seventh.Resource.Common.Classes.Options;
using Seventh.Resource.Services;

namespace Seventh.Resource
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

        public static void AddSeventhAssetServices(this IServiceCollection services, Action<ResourceOption> resourceOptionAction)
        {
            var resourceOption = new ResourceOption();
            resourceOptionAction(resourceOption);
            services.AddSingleton(p => new OptionService()
                .UseStatusOptions(resourceOption));
            services.AddSingleton<FileWatcherService>();
            services.AddSingleton<AssetDownloadService>();
        }
    }
}