using System;
using System.IO;
using System.Reflection;
using SeventhServices.Resource.Common;
using SeventhServices.Resource.Common.Classes;
using SeventhServices.Resource.Common.Classes.Options;
using PathOption = SeventhServices.Resource.Common.Classes.Options.PathOption;

namespace SeventhServices.Resource.Services
{
    public static class ServiceExtension
    {

        public static StatusService UseStatusOptions(this StatusService statusService, StatusOption statusOption)
        {
            if (statusOption.Account == null)
            {
                statusService.Account = new Account(SecretKey.Implement.DefaultEncPid, SecretKey.Implement.DefaultId);
            }
            if (statusService.PathOption == null)
            {
                statusService.PathOption = new PathOption(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location));
            }
            if (statusService.SortOption == null)
            {
                statusService.SortOption = new AssetSortOption();
            }
            return statusService.SetStatusOptions(statusOption);
        }

        public static StatusService ConfigureStatusOptions(this StatusService statusService, Action<StatusOption> statusOption)
        {
            var defaultStatusOption = new StatusOption
            {
                Rev = 0,
                Account = new Account(SecretKey.Implement.DefaultEncPid, SecretKey.Implement.DefaultId),
                PathOption = new PathOption(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)),
                SortOption = new AssetSortOption()
            };
            statusOption(defaultStatusOption);
            return statusService.SetStatusOptions(defaultStatusOption);
        }

        public static StatusService ConfigureStatusOptions(this StatusService statusService)
        {
            return ConfigureStatusOptions(statusService, o => { });
        }
    }
}