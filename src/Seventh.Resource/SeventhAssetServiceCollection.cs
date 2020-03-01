using System;
using Microsoft.Extensions.DependencyInjection;
using Seventh.Resource.Common.Options;
using Seventh.Resource.Services;

namespace Seventh.Resource
{
    public static class ServiceExtension
    {
        public static void AddSeventhResourceServices(this IServiceCollection services)
        {
            services.AddSingleton(p => new ResourceLocation()
                .ConfigureOptions());
            services.AddSingleton<FileWatcherService>();
            services.AddScoped<AssetDownloadService>();
            services.AddHttpClient<AssetDownloadClient>();
            services.AddSingleton<AssetSortService>();
        }

        public static void AddSeventhResourceServices(this IServiceCollection services, Action<ResourceOption> resourceOptionAction)
        {
            var resourceOption = new ResourceOption();
            resourceOptionAction(resourceOption);
            services.AddSingleton(p => new ResourceLocation()
                .UseStatusOptions(resourceOption));
            services.AddSingleton<FileWatcherService>();
            services.AddScoped<AssetDownloadService>();
            services.AddHttpClient<AssetDownloadClient>();
            services.AddSingleton<AssetSortService>();
        }
    }
}