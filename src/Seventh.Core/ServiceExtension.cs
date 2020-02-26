using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Seventh.Core
{
    public static class ServiceExtension
    {
        public static void AddSeventhCore(this IServiceCollection services)
        {
            services.AddSingleton<SeventhServiceLocation>();
            services.AddSingleton<SevenResourceService>();
            services.AddSingleton<SevenStatusService>();
        }
    }
}
