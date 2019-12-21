using Microsoft.Extensions.Logging;
using SeventhServices.Client.Common.Params;
using SeventhServices.Resource.Common.Classes;
using SeventhServices.Resource.Common.Classes.Options;
using SeventhServices.Resource.Common.Utilities;
using PathOption = SeventhServices.Resource.Common.Classes.Options.PathOption;
using SecretKey = SeventhServices.Resource.Common.SecretKey;

namespace SeventhServices.Resource.Services
{
    public class StatusService
    {
        private readonly ILogger<StatusService> _logger;

        public StatusService(ILoggerFactory logger)
        {
            _logger = logger.CreateLogger<StatusService>();
        }

        static StatusService()
        {
            ConfigureWatcher.TryAddConfigure<Status>();
            ConfigureWatcher.TryAddConfigure<SecretKey>();
        }

        public int Rev
        {
            get
            {
                Status = ConfigureWatcher.GetFreshConfigure<Status>();
                return Status.Revision;
            }
            set
            {
                Status = ConfigureWatcher.GetFreshConfigure<Status>();
                if (value <= Status.Revision) return;
                AssetVersionChanged(value);
                Status.Revision = value;
                ConfigureWatcher.RefreshConfigure<Status>(Status);
            }
        }

        public Account Account
        {
            get
            {
                Status = ConfigureWatcher.GetFreshConfigure<Status>();
                return Status.GetAccount();
            }
            set
            {
                Status = ConfigureWatcher.GetFreshConfigure<Status>();
                Status.SetAccount(value);
                ConfigureWatcher.RefreshConfigure<Status>(Status);
            }
        }

        public GameVersion GameVersion
        {
            get
            {
                Status = ConfigureWatcher.GetFreshConfigure<Status>();
                return Status.GetGameVersion();
            }
            set
            {
                if (value == null) return;
                Status = ConfigureWatcher.GetFreshConfigure<Status>();
                if (value <= Status.GetGameVersion()) return;
                GameVersionChanged(value);
                Status.SetGameVersion(value);
                ConfigureWatcher.RefreshConfigure<Status>(Status);
            }
        }

        private Status Status { get; set; }
        public AssetSortOption SortOption { get; set; }
        public PathOption PathOption { get; set; }

        public StatusService SetStatusOptions(StatusOption statusOption)
        {
            GameVersion = statusOption.GameVersion;
            Account = statusOption.Account;
            Rev = statusOption.Rev;
            PathOption = statusOption.PathOption;
            SortOption = statusOption.SortOption;
            CreatePath(PathOption);
            return this;
        }

        #region Event

        private void GameVersionChanged(GameVersion newVersion)
        {
            RequestParams.Version = newVersion.Version;
            _logger.LogInformation($"Game version update : " +
                                   $"{Status.Version} => {newVersion.Version}");
        }

        private void AssetVersionChanged(int rev)
        {
            _logger.LogInformation($"Asset version update : {Status.Revision} => {rev}");
        }
        #endregion

        private static void CreatePath(PathOption pathOption)
        {
            CommonUtil.CreateRequireDirectories(pathOption.ConfigurePath);
            CommonUtil.CreateRequireDirectories(pathOption.IndexPath);
            CommonUtil.CreateRequireDirectories(pathOption.AssetPath);
        }
    }
}