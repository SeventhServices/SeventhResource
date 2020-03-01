using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Seventh.Resource.Common.Options;
using Seventh.Resource.Services;

namespace Seventh.Resource
{
    public static class ServiceExtension
    {
        public static void AddSeventhResourceServices(this IServiceCollection services)
        {
            services.TryAddSingleton(p => new ResourceLocation()
                .ConfigureOptions());
            services.TryAddSingleton<FileWatcherService>();
            services.TryAddScoped<AssetDownloadService>();
            services.AddHttpClient<AssetDownloadClient>();
            services.TryAddSingleton<AssetSortService>();
            services.TryAddSingleton<AssetQueueDownloadService>();
        }

        public static void AddSeventhResourceServices(this IServiceCollection services, Action<ResourceOption> resourceOptionAction)
        {
            var resourceOption = new ResourceOption();
            resourceOptionAction(resourceOption);
            services.TryAddSingleton(p => new ResourceLocation()
                .UseStatusOptions(resourceOption));
            services.TryAddSingleton<FileWatcherService>();
            services.TryAddScoped<AssetDownloadService>();
            services.AddHttpClient<AssetDownloadClient>();
            services.TryAddSingleton<AssetSortService>();
            services.TryAddSingleton<AssetQueueDownloadService>();
        }
    }
}