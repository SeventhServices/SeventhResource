using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Seventh.Resource.Data.Abstractions;
using Seventh.Resource.Database;

namespace Seventh.Resource.Data
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddSeventhResourceData(this IServiceCollection services)
        {
            services.TryAddSingleton<ISqlLoader, SqlLoader>();
            return services;
        }
    }
}
