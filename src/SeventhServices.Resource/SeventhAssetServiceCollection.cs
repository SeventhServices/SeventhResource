using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SeventhServices.Client.Extensions.DependencyInjection;
using SeventhServices.Client.Extensions.HttpClientFactory;
using SeventhServices.Resource.Services;

namespace SeventhServices.Resource
{
    public static class SeventhAssetServiceCollection
    {
        public static void AddSeventhAssetServices(this IServiceCollection services)
        {
            services.AddSeventhRequireHttpFactoryApi();
            services.AddSingleton<AccountService>();
            services.AddSingleton<GameVersionCheckService>();
            services.AddSingleton<AssetVersionCheckService>();
            services.AddSingleton(p => new StatusService(
                    p.GetRequiredService<ILoggerFactory>())
                .ConfigureStatusOptions());
            services.AddSingleton<AssetCryptService>();
            services.AddSingleton<FileWatcherService>();
            services.AddSingleton<AssetDownloadService>();
        }

        public static void AddSeventhAssetServices(this IServiceCollection services, Action<SeventhServicesOptions> seventhServicesOptions)
        {
            var defaultSeventhServicesOption = new SeventhServicesOptions();
            seventhServicesOptions(defaultSeventhServicesOption);
            if (defaultSeventhServicesOption.UseHttpFactory)
            {
                services.AddSeventhRequireHttpFactoryApi();
            }
            else
            {
                services.AddSeventhRequireHttpApi();
            }
            services.AddSingleton<AccountService>();
            services.AddSingleton<GameVersionCheckService>();
            services.AddSingleton<AssetVersionCheckService>();
            services.AddSingleton(p => new StatusService(
                    p.GetRequiredService<ILoggerFactory>())
                .UseStatusOptions(defaultSeventhServicesOption.StatusOption));
            services.AddSingleton<AssetCryptService>();
            services.AddSingleton<FileWatcherService>();
            services.AddSingleton<AssetDownloadService>();
        }
    }
}