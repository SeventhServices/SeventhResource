using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Seventh.Resource.Common.Options;
using Seventh.Resource.Data.Abstractions;
using Seventh.Resource.Database;
using Seventh.Resource.Services;

namespace Seventh.Resource
{
    public static class ServiceExtension
    {
        public static void AddSeventhResourceServices(this IServiceCollection services)
        {
            services.TryAddSingleton(p => new ResourceLocation()
                .ConfigureLocation());
            services.TryAddSingleton<FileWatcherService>();
            services.AddHttpClient<DownloadClient>();
            services.TryAddScoped<DownloadService>();
            services.AddHttpClient<OneByOneDownloadClient>();
            services.TryAddScoped<OneByOneDownloadService>();
            services.TryAddSingleton<SortService>();
            services.TryAddSingleton<QueueDownloadService>();
            services.TryAddSingleton<AssetInfoService>();
            services.TryAddSingleton<ISqlLoader, SqlLoader>();
        }

        public static void AddSeventhResourceServices(this IServiceCollection services, Action<ResourceOption> resourceOptionAction)
        {
            var resourceOption = new ResourceOption();
            resourceOptionAction(resourceOption);
            services.TryAddSingleton(p => new ResourceLocation()
                .UseStatusOptions(resourceOption));
            services.TryAddSingleton<FileWatcherService>();
            services.TryAddScoped<DownloadService>();
            services.AddHttpClient<DownloadClient>();
            services.AddHttpClient<OneByOneDownloadClient>();
            services.TryAddScoped<OneByOneDownloadService>();
            services.TryAddSingleton<SortService>();
            services.TryAddSingleton<QueueDownloadService>();
            services.TryAddSingleton<AssetInfoService>();
            services.TryAddSingleton<ISqlLoader, SqlLoader>();
        }
    }
}