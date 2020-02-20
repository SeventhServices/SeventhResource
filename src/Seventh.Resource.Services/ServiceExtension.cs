using System;
using System.IO;
using System.Reflection;
using Seventh.Resource.Common;
using Seventh.Resource.Common.Classes;
using Seventh.Resource.Common.Classes.Options;
using PathOption = Seventh.Resource.Common.Classes.Options.PathOption;

namespace Seventh.Resource.Services
{
    public static class ServiceExtension
    {

        public static OptionService UseStatusOptions(this OptionService optionService, ResourceOption options)
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

        public static OptionService ConfigureStatusOptions(this OptionService optionService, Action<ResourceOption> statusOption)
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

        public static OptionService ConfigureOptions(this OptionService optionService)
        {
            return ConfigureStatusOptions(optionService, o => {});
        }
    }
}