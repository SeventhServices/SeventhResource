using System;
using System.IO;
using System.Reflection;
using Seventh.Resource.Common;
using Seventh.Resource.Common.Entities;
using Seventh.Resource.Common.Options;
using PathOption = Seventh.Resource.Common.Options.PathOption;

namespace Seventh.Resource.Services
{
    public static class ServiceExtension
    {

        public static ResourceLocation UseStatusOptions(this ResourceLocation optionService, ResourceOption options)
        {
            if (options.PathOption == null)
            {
                options.PathOption = new PathOption(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location));
            }
            if (options.SortOption == null)
            {
                options.SortOption = new SortOptions();
            }
            return optionService.SetOptions(options);
        }

        public static ResourceLocation ConfigureLocation(this ResourceLocation optionService, Action<ResourceOption> statusOption)
        {
            var defaultStatusOption = new ResourceOption
            {
                Account = new Account(SecretKey.Implement.DefaultEncPid, SecretKey.Implement.DefaultId),
                PathOption = new PathOption(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)),
                SortOption = new SortOptions()
            };
            statusOption(defaultStatusOption);
            return optionService.SetOptions(defaultStatusOption);
        }

        public static ResourceLocation ConfigureLocation(this ResourceLocation optionService)
        {
            return ConfigureLocation(optionService, o => {});
        }
    }
}