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
            if (optionService.PathOption == null)
            {
                optionService.PathOption = new PathOption(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location));
            }
            if (optionService.SortOption == null)
            {
                optionService.SortOption = new AssetSortOption();
            }
            return optionService.SetOptions(options);
        }

        public static ResourceLocation ConfigureLocation(this ResourceLocation optionService, Action<ResourceOption> statusOption)
        {
            var defaultStatusOption = new ResourceOption
            {
                Account = new Account(SecretKey.Implement.DefaultEncPid, SecretKey.Implement.DefaultId),
                PathOption = new PathOption(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)),
                SortOption = new AssetSortOption()
            };
            statusOption(defaultStatusOption);
            return optionService.SetOptions(defaultStatusOption);
        }

        public static ResourceLocation ConfigureOptions(this ResourceLocation optionService)
        {
            return ConfigureLocation(optionService, o => {});
        }
    }
}