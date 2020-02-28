using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Seventh.Core.Services;

namespace Seventh.Core
{
    public static class ServiceExtension
    {
        public static void AddSeventhCore(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.TryAddSingleton<SeventhServiceLocation>();
            services.TryAddScoped<SevenResourceService>();
            services.TryAddScoped<SevenStatusService>();
        }
    }
}
