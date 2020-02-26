using System;
using Microsoft.Extensions.DependencyInjection;
using Seventh.Core;
using Seventh.Resource.Common.Classes.Options;
using Seventh.Resource.Services;

namespace Seventh.Resource
{
    public static class ServiceExtension
    {
        public static void AddSeventhResourceServices(this IServiceCollection services)
        {
            services.AddSeventhCore();
            services.AddSingleton(p => new ResourceLocation()
                .ConfigureOptions());
            services.AddSingleton<FileWatcherService>();
            services.AddScoped<AssetDownloadService>();
            services.AddHttpClient<AssetDownloadClient>();
            services.AddSingleton<AssetSortService>();

            services.AddSingleton<SeventhServiceLocation>();
            services.AddSingleton<SevenResourceService>();
            services.AddSingleton<SevenStatusService>();
        }

        public static void AddSeventhResourceServices(this IServiceCollection services, Action<ResourceOption> resourceOptionAction)
        {
            services.AddSeventhCore();
            var resourceOption = new ResourceOption();
            resourceOptionAction(resourceOption);
            services.AddSingleton(p => new ResourceLocation()
                .UseStatusOptions(resourceOption));
            services.AddSingleton<FileWatcherService>();
            services.AddScoped<AssetDownloadService>();
            services.AddHttpClient<AssetDownloadClient>();
            services.AddSingleton<AssetSortService>();

            services.AddSingleton<SeventhServiceLocation>();
            services.AddSingleton<SevenResourceService>();
            services.AddSingleton<SevenStatusService>();
        }
    }
}