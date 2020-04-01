using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Seventh.Resource.Api.Data.Abstractions;
using Seventh.Resource.Api.Data.Repository;
using Seventh.Resource.Database.Entity;

namespace Seventh.Resource.Api.Data
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddDataProvider(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.TryAddSingleton<IDataCachePool, MemoryDataCachePool>();
            services.TryAddScoped<IContextProvider, ContextProvider>();
            services.TryAddScoped<IRepository<Card>, CardRepository>();
            services.TryAddScoped<IRepository<Character>, CharacterRepository>();
            return services;
        }
    }
}
