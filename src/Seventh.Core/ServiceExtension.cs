using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Seventh.Core.Abstractions.Extend;
using Seventh.Core.Extend;
using Seventh.Core.Services;

namespace Seventh.Core
{
    public static class ServiceExtension
    {
        public static void AddSeventhCore(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.TryAddScoped<IHttpExtend, HttpExtend>();
            services.TryAddScoped<IJsonHttpExtend, JsonHttpExtend>();
            services.TryAddSingleton(p =>
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            services.TryAddSingleton<SeventhServiceLocation>();
            services.TryAddScoped<SeventhResourceService>();
            services.TryAddScoped<SeventhStatusService>();
        }
    }
}
